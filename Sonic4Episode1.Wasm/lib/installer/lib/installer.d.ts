/// <reference types="emscripten" />
export type Progress = {
    status: string;
    progress: number;
    step?: number;
};
type ProgressCallback = (progress: Progress) => void;
declare class Installer {
    private _FS;
    private _IDBFS;
    private _callback;
    constructor(_FS: typeof FS, _IDBFS: typeof IDBFS, _callback: ProgressCallback);
    private exists;
    private join;
    private serverVersion;
    private version;
    check: () => Promise<{
        version: string;
        serverVersion: string;
    }>;
    install: () => Promise<void>;
}
export default function createSetup(_FS: typeof FS, _IDBFS: typeof IDBFS, callback: ProgressCallback): Promise<Installer>;
export {};
//# sourceMappingURL=installer.d.ts.map