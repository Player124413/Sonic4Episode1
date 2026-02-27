import * as asar from 'asar/dist/extractor.js';
const CHECKING_FOR_UPDATES = 'Checking for updates...';
const DOWNLOADING_UPDATE_PACKAGE = 'Downloading update package...';
const INSTALLING_UPDATE_PACKAGE = 'Installing update package...';
class Installer {
    constructor(_FS, _IDBFS, _callback) {
        this._FS = _FS;
        this._IDBFS = _IDBFS;
        this._callback = _callback;
        this.exists = (path) => this._FS.analyzePath(path).exists;
        this.join = (dir, file) => `${dir}/${file}`;
        this.serverVersion = '';
        this.version = '';
        this.check = async () => {
            this._callback({ status: CHECKING_FOR_UPDATES, progress: 0, step: 0 });
            const version = this.exists('/content/version.txt')
                ? this._FS.readFile('/content/version.txt', { encoding: 'utf8' })
                : '0';
            console.log(`Content version: ${version}`);
            const serverVersion = await (await fetch('version.txt')).text();
            console.log(`Server version: ${serverVersion}`);
            this._callback({ status: CHECKING_FOR_UPDATES, progress: 100, step: 0 });
            this.version = version;
            this.serverVersion = serverVersion;
            return { version, serverVersion };
        };
        this.install = async () => {
            this._callback({ status: DOWNLOADING_UPDATE_PACKAGE, progress: 0, step: 1 });
            let request = new XMLHttpRequest();
            request.open('GET', 'Content.asar', true);
            request.responseType = '';
            request.onprogress = (e) => {
                if (e.lengthComputable) {
                    this._callback({ status: DOWNLOADING_UPDATE_PACKAGE, progress: e.loaded / e.total, step: 1 });
                }
            };
            const asarBlob = await new Promise((res, rej) => {
                request.onload = () => res(request.response);
                request.send();
            });
            this._callback({ status: INSTALLING_UPDATE_PACKAGE, progress: 0, step: 2 });
            const entries = await asar.extractAll(asarBlob, { flat: true });
            const length = Object.keys(entries).length;
            let i = 0;
            for (const [entry, data] of Object.entries(entries)) {
                if (!entry)
                    continue;
                const entryPath = entry.toLowerCase().split('/');
                const entryName = entryPath[entryPath.length - 1];
                const entryDir = entryPath.slice(0, entryPath.length - 1).join('/');
                if (entryDir) {
                    const pieces = entryDir.split('/').filter(piece => piece !== '');
                    let current = '';
                    for (const piece of pieces) {
                        current = current + '/' + piece;
                        if (this.exists(current))
                            continue;
                        this._FS.mkdir(current);
                    }
                }
                if (entryName) {
                    const file = this.join(entryDir, entryName);
                    if (this.exists(file)) {
                        this._FS.unlink(file);
                    }
                    this._FS.writeFile(file, new Uint8Array(data));
                    this._callback({ status: INSTALLING_UPDATE_PACKAGE, progress: i / length, step: 2 });
                }
                if (i % 15 === 0) {
                    // yield a tick
                    await new Promise((res, rej) => {
                        setTimeout(() => res(), 0);
                    });
                }
                i++;
            }
            this._FS.writeFile('/content/version.txt', this.serverVersion);
            await new Promise((res, rej) => {
                this._FS.syncfs(false, () => res());
            });
        };
    }
}
export default function createSetup(_FS, _IDBFS, callback) {
    return new Promise((res, rej) => {
        _FS.mkdir('/content');
        _FS.mount(_IDBFS, {}, '/content');
        _FS.syncfs(true, () => {
            res(new Installer(_FS, _IDBFS, callback));
        });
    });
}
//# sourceMappingURL=installer.js.map