// Which content tree this device gets. Two independent choices: the set, which the player picks,
// and the texture format, which the hardware decides. See docs/webgl-texture-capabilities.md.

export type ContentSet = 'sd' | 'hd' | 'console';

const STORAGE_KEY = 'contentSet';

// [bc, astc] per set. There is no mobile-sd-dxt: PNG costs GPU memory rather than download, which
// is the cheaper thing to spend on the smallest tree
const PROFILES: Record<ContentSet, [string, string]> = {
    sd: ['mobile-sd-png', 'mobile-sd-astc'],
    hd: ['mobile-hd-dxt', 'mobile-hd-astc'],
    console: ['desktop-hd-dxt', 'desktop-hd-astc'],
};

// an extension can be listed and its enums still rejected, so this uploads one real block rather
// than trusting getSupportedExtensions
function supportsAstc(): boolean {
    const canvas = document.createElement('canvas');
    const gl = canvas.getContext('webgl2') as WebGL2RenderingContext | null;
    if (!gl) return false;

    const extension = gl.getExtension('WEBGL_compressed_texture_astc');
    if (!extension) return false;

    const COMPRESSED_RGBA_ASTC_6x6_KHR = 0x93B4;
    const texture = gl.createTexture();
    gl.bindTexture(gl.TEXTURE_2D, texture);
    while (gl.getError() !== gl.NO_ERROR) { /* drain */ }
    gl.compressedTexImage2D(gl.TEXTURE_2D, 0, COMPRESSED_RGBA_ASTC_6x6_KHR, 6, 6, 0, new Uint8Array(16));
    const ok = gl.getError() === gl.NO_ERROR;
    gl.deleteTexture(texture);

    return ok;
}

export function remembered(): ContentSet {
    try {
        const stored = localStorage.getItem(STORAGE_KEY);
        if (stored === 'sd' || stored === 'hd' || stored === 'console') return stored;
    } catch { /* private browsing */ }

    return 'hd';
}

export function remember(set: ContentSet) {
    try {
        localStorage.setItem(STORAGE_KEY, set);
    } catch { /* private browsing */ }
}

export function profileFor(set: ContentSet): string {
    const [bc, astc] = PROFILES[set];
    return supportsAstc() ? astc : bc;
}

export async function sizes(): Promise<Record<ContentSet, number>> {
    const astc = supportsAstc();
    const entries = await Promise.all((Object.keys(PROFILES) as ContentSet[]).map(async set => {
        const profile = PROFILES[set][astc ? 1 : 0];
        try {
            const response = await fetch(`sets/${profile}.tar.gz`, { method: 'HEAD' });
            return [set, Number(response.headers.get('content-length') ?? 0)] as const;
        } catch {
            return [set, 0] as const;
        }
    }));

    return Object.fromEntries(entries) as Record<ContentSet, number>;
}

// the picker doubles as the gesture WebAudio needs, so it is always shown, with the last choice
// preselected rather than skipped
export function choose(): Promise<ContentSet> {
    const overlay = document.querySelector('#tap-to-play') as HTMLElement;
    const go = document.querySelector('#go') as HTMLButtonElement;

    const last = remembered();
    const preselected = document.querySelector(
        `input[name="content"][value="${last}"]`) as HTMLInputElement | null;
    if (preselected) preselected.checked = true;

    sizes().then(found => {
        for (const [set, bytes] of Object.entries(found)) {
            const label = document.querySelector(`#size-${set}`);
            if (label && bytes > 0) label.textContent = `${(bytes / 1048576).toFixed(0)} MB`;
        }
    });

    overlay.classList.add('visible');

    return new Promise<ContentSet>(resolve => {
        go.addEventListener('click', () => {
            const picked = document.querySelector(
                'input[name="content"]:checked') as HTMLInputElement;
            const set = (picked?.value ?? 'hd') as ContentSet;

            remember(set);
            overlay.classList.remove('visible');
            resolve(set);
        }, { once: true });
    });
}
