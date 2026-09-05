// Bundles come down as gzipped tars and their members go into CacheStorage one file each, so the
// game only ever pays for the file it asked for. See docs/wasm-content-delivery-plan.md

type Fs = {
    mkdir(path: string): void;
    writeFile(path: string, data: Uint8Array): void;
    readFile(path: string, opts: { encoding: 'binary' }): Uint8Array;
};

type Progress = (status: string, fraction: number) => void;

// views over a plain ArrayBuffer, which is what Response accepts. The looser default generic
// admits SharedArrayBuffer and does not
type Bytes = Uint8Array<ArrayBuffer>;

type Bundle = {
    file: string;
    members: Map<string, Bytes>;
    written: Promise<unknown>;
};

const CONTENT_ROOT = '/content';
const CACHE_PREFIX = 's4e1-content-';
const ENTRY_PREFIX = '/content-cache/';
const MAX_ATTEMPTS = 4;
const BLOCK = 512;

let fs: Fs;
let profile = '';
let progress: Progress;
let cache: Cache | null = null;

const bundleOf = new Map<string, string>();
const boot = new Set<string>();
const inflight = new Map<string, Promise<Bundle>>();
let mru: Bundle | null = null;

// match and body are timed apart because the whole read is wall clock across an await, and on one
// thread that includes whatever the game did in between. A lookup that is slow in the same way the
// body read is slow is scheduling, not disk
const hits = { mru: 0, samples: [] as { match: number; body: number; bytes: number }[] };
const ingests: { at: number; file: string; trigger: string; ms: number; bytes: number }[] = [];

const started = performance.now();

function log(text: string) {
    const at = Math.round(performance.now() - started).toString().padStart(6);
    console.log(`[content ${at} ms] ${text}`);
}

function message(error: unknown): string {
    return error instanceof Error ? error.message : String(error);
}

// one rule for every lookup: forward slashes, lower case. The boot tree is laid out the same way
function key(path: string): string {
    return path.replace(/\\/g, '/').toLowerCase();
}

function url(file: string): string {
    return `sets/${profile}/${file}`;
}

export async function install(host: Fs, chosen: string, report: Progress): Promise<void> {
    fs = host;
    profile = chosen;
    progress = report;

    progress('Downloading...', 0);

    // boot is the long pole and its name never changes, so it goes first and the manifest and
    // cache open overlap with it
    const bootFetch = fetchBundle('boot.tar.gz');
    const manifest = await withRetry('manifest.txt', async () => (await fetchOk(url('manifest.txt'))).text());
    const version = parseManifest(manifest);
    log(`manifest: ${bundleOf.size} paths`);

    const [members] = await Promise.all([bootFetch, openCache(version)]);
    layOutBoot(members);

    progress('Preparing to launch...', 1);
}

// "version <hash>", "boot <file> <bytes>", then "bundle <name> <bytes>" followed by its paths
function parseManifest(text: string): string {
    let version = '';
    let current: string | null = null;

    for (const raw of text.split('\n')) {
        const line = raw.trim();
        if (line.length === 0) continue;

        const [word, ...rest] = line.split(' ');
        if (word === 'version') {
            version = rest[0];
        } else if (word === 'boot') {
            continue;
        } else if (word === 'bundle') {
            current = `${rest[0]}.tar.gz`;
        } else if (current !== null) {
            bundleOf.set(key(line), current);
        } else {
            throw new Error(`manifest: '${line}' before any bundle`);
        }
    }

    if (version.length === 0) throw new Error('manifest names no version');
    return version;
}

// no secure context, no caches. That is plain http on a LAN during development and nothing else.
// Anything else going wrong here costs speed, not correctness, so it degrades instead of throwing
async function openCache(version: string): Promise<void> {
    if (typeof caches === 'undefined') {
        log('CacheStorage unavailable, bundles will not persist');
        return;
    }

    try {
        const name = CACHE_PREFIX + version;
        const stale = (await caches.keys()).filter(k => k.startsWith(CACHE_PREFIX) && k !== name);
        await Promise.all(stale.map(k => caches.delete(k)));
        cache = await caches.open(name);

        if (stale.length > 0) log(`dropped ${stale.length} stale content cache(s)`);
    } catch (error) {
        cache = null;
        log(`CacheStorage unusable (${message(error)}), bundles will not persist`);
    }
}

// the browser can drop the whole origin's storage whenever it likes, which makes an open Cache
// reject rather than miss. Every byte behind it is refetchable, so a failure here is a miss
async function fromCache(k: string): Promise<Bytes | undefined> {
    if (cache === null) return undefined;

    const at = performance.now();
    try {
        const hit = await cache.match(ENTRY_PREFIX + k);
        if (hit === undefined) return undefined;

        const found = performance.now();
        const bytes = new Uint8Array(await hit.arrayBuffer());

        hits.samples.push({
            match: found - at,
            body: performance.now() - found,
            bytes: bytes.length,
        });

        return bytes;
    } catch (error) {
        log(`cache read of '${k}' failed (${message(error)}), refetching`);
        return undefined;
    }
}

const made = new Set<string>();

function ensureDir(dir: string) {
    if (made.has(dir)) return;

    const parent = dir.substring(0, dir.lastIndexOf('/'));
    if (parent.length > 0) ensureDir(parent);

    try {
        fs.mkdir(dir);
    } catch {
        // already there
    }
    made.add(dir);
}

function layOutBoot(members: Map<string, Bytes>) {
    const at = performance.now();
    let bytes = 0;

    for (const [k, data] of members) {
        const path = `${CONTENT_ROOT}/${k}`;
        ensureDir(path.substring(0, path.lastIndexOf('/')));
        fs.writeFile(path, data);
        boot.add(k);
        bytes += data.length;
    }

    log(`boot laid out: ${members.size} files, ${Math.round(bytes / 1024)} KB in ${Math.round(performance.now() - at)} ms`);
}

// the runtime cannot take a Task<byte[]>, so a read is two calls: the length, then a copy into the
// buffer the managed side allocated. Queued per path because BGM prepare and amFs can both have a
// read in flight, and a single slot would be overwritten between one resolving and its take
const ready = new Map<string, Uint8Array[]>();

export async function readBegin(path: string): Promise<number> {
    const bytes = await read(path);

    let queue = ready.get(path);
    if (queue === undefined) {
        queue = [];
        ready.set(path, queue);
    }
    queue.push(bytes);

    return bytes.length;
}

export function readTake(path: string, into: { set(source: Uint8Array): void }): void {
    const queue = ready.get(path);
    const bytes = queue?.shift();
    if (bytes === undefined)
        throw new Error(`readTake('${path}') without a matching readBegin`);

    if (queue!.length === 0) ready.delete(path);
    into.set(bytes);
}

async function read(path: string): Promise<Uint8Array> {
    const k = key(path);

    if (boot.has(k))
        return fs.readFile(`${CONTENT_ROOT}/${k}`, { encoding: 'binary' });

    const file = bundleOf.get(k);
    if (file === undefined)
        return fetchStreamed(path);

    if (mru !== null && mru.file === file) {
        const hit = mru.members.get(k);
        if (hit !== undefined) {
            hits.mru++;
            return hit;
        }
    }

    const cached = await fromCache(k);
    if (cached !== undefined) return cached;

    const bundle = await ingest(file, k);
    const bytes = bundle.members.get(k);
    if (bytes === undefined)
        throw new Error(`'${path}' is not in ${file}`);

    return bytes;
}

// BGM, a track at a time. Anything that is neither boot nor bundled is served loose under Content/
async function fetchStreamed(path: string): Promise<Uint8Array> {
    const response = await withRetry(path, () => fetchOk(`Content/${path}`));
    return new Uint8Array(await response.arrayBuffer());
}

function ingest(file: string, trigger: string): Promise<Bundle> {
    const pending = inflight.get(file);
    if (pending !== undefined) return pending;

    // removed on rejection as well as completion, or the first failure poisons every later attempt
    const task = ingestOnce(file, trigger).finally(() => inflight.delete(file));
    inflight.set(file, task);
    return task;
}

async function ingestOnce(file: string, trigger: string): Promise<Bundle> {
    const at = performance.now();
    const members = await fetchBundle(file);
    const fetched = performance.now();

    // the previous bundle stays the MRU until its writes have settled, so at every moment a file is
    // in the MRU, fully in the cache, or never ingested
    if (mru !== null) await mru.written;

    const written = cache !== null ? writeAll(file, members) : Promise.resolve();
    mru = { file, members, written };

    let bytes = 0;
    for (const data of members.values()) bytes += data.length;
    log(`${file}: ${members.size} files / ${Math.round(bytes / 1024)} KB in ${Math.round(fetched - at)} ms`);

    ingests.push({ at, file, trigger, ms: fetched - at, bytes });
    return mru;
}

function writeAll(file: string, members: Map<string, Bytes>): Promise<unknown> {
    const puts: Promise<void>[] = [];
    for (const [k, data] of members)
        puts.push(cache!.put(ENTRY_PREFIX + k, new Response(data)));

    return Promise.allSettled(puts).then(results => {
        const failed = results.filter(r => r.status === 'rejected').length;
        if (failed > 0) log(`${file}: ${failed} of ${puts.length} cache writes failed, members will re-download`);
    });
}

async function fetchBundle(file: string): Promise<Map<string, Bytes>> {
    return withRetry(file, async () => {
        const response = await fetchOk(url(file));
        const buffer = await new Response(await tarStream(response)).arrayBuffer();
        return untar(new Uint8Array(buffer));
    });
}

let sniffed = false;

// a host that sets Content-Encoding has already had the browser decode this, so what arrives is
// the tar. A host that serves the file as an opaque download hands back what we shipped. The two
// are told apart by gzip's magic rather than by Content-Encoding, which is not worth trusting
// across every way this gets served
async function tarStream(response: Response): Promise<ReadableStream<Bytes>> {
    const reader = response.body!.getReader();
    const head: Bytes[] = [];
    let held = 0;

    while (held < 2) {
        const next = await reader.read();
        if (next.done) break;

        head.push(next.value);
        held += next.value.length;
    }

    const rest = new ReadableStream<Bytes>({
        start(controller) {
            for (const chunk of head) controller.enqueue(chunk);
        },
        async pull(controller) {
            const next = await reader.read();
            if (next.done) controller.close();
            else controller.enqueue(next.value);
        },
        cancel: reason => reader.cancel(reason),
    });

    const compressed = held >= 2 && head[0][0] === 0x1f && (head[0][1] ?? head[1][0]) === 0x8b;
    if (!sniffed) {
        sniffed = true;
        log(compressed ? 'bundles arrive gzipped, inflating here' : 'bundles arrive decoded by the browser');
    }

    return compressed ? rest.pipeThrough(new DecompressionStream('gzip')) : rest;
}

class HttpError extends Error {
    constructor(readonly status: number, message: string) {
        super(message);
    }
}

async function fetchOk(target: string): Promise<Response> {
    const response = await fetch(target);
    if (!response.ok)
        throw new HttpError(response.status, `${target}: ${response.status} ${response.statusText}`);

    return response;
}

// a 404 is a packaging mistake, and retrying it would turn a loud build error into a hang. A
// TypeError is fetch's word for the network, whether the request never left or the body dropped
function isTransient(error: unknown): boolean {
    if (error instanceof HttpError)
        return error.status === 408 || error.status === 429 || error.status >= 500;

    return error instanceof TypeError;
}

async function withRetry<T>(what: string, once: () => Promise<T>): Promise<T> {
    for (let attempt = 1; ; attempt++) {
        try {
            return await once();
        } catch (error) {
            if (attempt >= MAX_ATTEMPTS || !isTransient(error)) throw error;

            const backoff = 250 << (attempt - 1);
            log(`${what}: attempt ${attempt} failed (${message(error)}), retrying in ${backoff} ms`);
            await new Promise(resolve => setTimeout(resolve, backoff));
        }
    }
}

// what a session actually fetched and in what order, which is where the manifest's prefetch groups
// come from. Ingests closer together than this are one load barrier rather than two
const GROUP_GAP_MS = 3000;

function quantile(sorted: number[], fraction: number): number {
    if (sorted.length === 0) return 0;
    return sorted[Math.min(sorted.length - 1, Math.floor(sorted.length * fraction))];
}

function spread(label: string, values: number[]): string {
    const sorted = [...values].sort((a, b) => a - b);
    const total = sorted.reduce((s, v) => s + v, 0);

    return `${label} mean ${(total / sorted.length).toFixed(1)} ms,`
        + ` p50 ${quantile(sorted, 0.5).toFixed(1)},`
        + ` p90 ${quantile(sorted, 0.9).toFixed(1)},`
        + ` max ${quantile(sorted, 1).toFixed(1)}`;
}

export function report(): string {
    const lines: string[] = [];
    const bytes = hits.samples.reduce((s, h) => s + h.bytes, 0);

    lines.push(`profile ${profile}, cache ${cache === null ? 'off' : 'on'}`);
    lines.push(`cache hits ${hits.samples.length}, ${Math.round(bytes / 1048576)} MB`);

    if (hits.samples.length > 0) {
        lines.push(`  ${spread('lookup', hits.samples.map(h => h.match))}`);
        lines.push(`  ${spread('body  ', hits.samples.map(h => h.body))}`);

        const bodyMs = hits.samples.reduce((s, h) => s + h.body, 0);
        lines.push(`  body throughput ${(bytes / 1048576 / (bodyMs / 1000)).toFixed(0)} MB/s`);

        // if body time tracks size this is disk, if it is flat across the buckets it is a fixed
        // cost per read and the only way to spend less of it is to do fewer, larger, overlapped
        for (const [label, low, high] of [
            ['<128 KB  ', 0, 131072],
            ['128K-1 MB', 131072, 1048576],
            ['>1 MB    ', 1048576, Infinity],
        ] as [string, number, number][]) {
            const inBucket = hits.samples.filter(h => h.bytes >= low && h.bytes < high);
            if (inBucket.length === 0) continue;

            const meanMs = inBucket.reduce((s, h) => s + h.body, 0) / inBucket.length;
            const meanKb = inBucket.reduce((s, h) => s + h.bytes, 0) / inBucket.length / 1024;
            lines.push(`  ${label} n=${inBucket.length.toString().padStart(3)} mean ${meanMs.toFixed(1)} ms for ${meanKb.toFixed(0)} KB`);
        }
    }

    lines.push(`mru hits ${hits.mru}`);
    lines.push(`ingests ${ingests.length}, ${Math.round(ingests.reduce((s, i) => s + i.bytes, 0) / 1048576)} MB`);
    lines.push('');

    let previous = -Infinity;
    for (const ingest of ingests) {
        if (ingest.at - previous > GROUP_GAP_MS) lines.push('--');
        previous = ingest.at;

        const at = (ingest.at / 1000).toFixed(1).padStart(7);
        lines.push(`${at}s  ${ingest.file.replace('.tar.gz', '').padEnd(18)} ${Math.round(ingest.ms).toString().padStart(5)} ms  ${ingest.trigger}`);
    }

    return lines.join('\n');
}

function ascii(block: Uint8Array, offset: number, length: number): string {
    let end = offset;
    while (end < offset + length && block[end] !== 0) end++;
    return String.fromCharCode(...block.subarray(offset, end));
}

function isZero(block: Uint8Array): boolean {
    for (const b of block) if (b !== 0) return false;
    return true;
}

// members are views over the one decompressed buffer, so the buffer is the MRU and nothing is
// copied until a file crosses into the runtime
function untar(data: Bytes): Map<string, Bytes> {
    const files = new Map<string, Bytes>();
    let at = 0;

    while (at < data.length) {
        if (data.length - at < BLOCK)
            throw new Error(`archive ends ${data.length - at} bytes into a header after ${files.size} files`);

        const header = data.subarray(at, at + BLOCK);
        at += BLOCK;

        // the archive ends in zero filled blocks, and type byte 0 means regular file, so a zero
        // block otherwise parses as a nameless empty file
        if (isZero(header)) break;

        let name = ascii(header, 0, 100);
        if (name.length === 0)
            throw new Error(`nameless entry after ${files.size} files`);

        // NUL or space terminated, both legal. GNU tar writes NUL, libarchive writes a space
        const sizeText = ascii(header, 124, 12).trim();
        const size = sizeText.length === 0 ? 0 : parseInt(sizeText, 8);
        const type = header[156];

        if (at + size > data.length)
            throw new Error(`archive ends inside '${name}'`);

        if (type === 0 || type === 48) {
            if (name.startsWith('./')) name = name.substring(2);
            files.set(key(name), data.subarray(at, at + size));
        }

        at += size + ((BLOCK - (size % BLOCK)) % BLOCK);
    }

    return files;
}
