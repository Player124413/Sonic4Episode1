// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
import Guide from './guide.js';
import WasmMediaPlayer from './mediaplayer.js';
// @ts-ignore
import { dotnet } from '../_framework/dotnet.js';
const loader = document.querySelector("#uno-body");
const progress = document.querySelector('#progress');
const status = document.querySelector('#status');
const canvas = document.querySelector('#canvas');
const tapToPlay = document.querySelector('#tap-to-play');
const updateProgress = (p) => {
    status.textContent = p.status;
    progress.value = (p.progress / 3) + (p.step / 3);
};
const resize = () => {
    const width = window.innerWidth;
    const height = window.innerHeight;
    const scale = Math.min(width / 800, height / 480);
    canvas.style.width = `${800 * scale}px`;
    canvas.style.height = `${480 * scale}px`;
    canvas.style.marginTop = `${(height - 480 * scale) / 2}px`;
};
const onInstall = () => {
    var _a;
    if ('umami' in window) {
        (_a = window.umami) === null || _a === void 0 ? void 0 : _a.track('App installed');
    }
};
window.onresize = resize;
window.addEventListener('appinstalled', onInstall);
const { setModuleImports, getAssemblyExports, getConfig } = await dotnet
    .withDiagnosticTracing(false)
    .withApplicationArguments(window.location.href)
    .create();
const FS = dotnet.instance.Module.FS;
const IDBFS = dotnet.instance.Module.IDBFS;
const S4E1 = await getAssemblyExports("Sonic4Episode1.Wasm.dll");
const mediaPlayer = new WasmMediaPlayer(S4E1);
setModuleImports('main.js', {
    setMainLoop: (cb) => dotnet.instance.Module.setMainLoop(cb),
    go: () => {
        loader.style.display = "none";
        tapToPlay.style.display = "none";
        resize();
        canvas.focus();
    },
    syncFS: () => new Promise((res, rej) => {
        FS.syncfs(false, (err) => {
            if (err)
                rej(err);
            res();
        });
    }),
    reportProgress: (str, prog) => {
        status.textContent = str;
        progress.value = prog;
    },
    MediaPlayer_GetVolume: mediaPlayer.getVolume.bind(mediaPlayer),
    MediaPlayer_SetVolume: mediaPlayer.setVolume.bind(mediaPlayer),
    MediaPlayer_Play: (song) => mediaPlayer.play(song),
    MediaPlayer_PlayLooped: (song, loop) => mediaPlayer.play(song, loop),
    MediaPlayer_LoadSong: (song) => mediaPlayer.preload(song),
    MediaPlayer_Stop: mediaPlayer.stop.bind(mediaPlayer),
    XmlStorage_GetSaveFile: () => localStorage.getItem('saveData'),
    XmlStorage_SetSaveFile: (data) => localStorage.setItem('saveData', data),
    SaveState_GetSaveState: () => localStorage.getItem('saveState'),
    SaveState_SetSaveState: (data) => localStorage.setItem('saveState', data),
    SaveState_HasSaveState: () => localStorage.getItem('saveState') != null,
    SaveState_DeleteSaveState: () => localStorage.removeItem('saveState'),
    Guide_BeginShowMessageBox: Guide.beginShowMessageBox.bind(Guide),
    WebBrowserTask_Open: (url) => window.open(url, '_blank'),
});
FS.mkdir('/content');
FS.mount(IDBFS, {}, '/content');
FS.syncfs(true, async () => {
    dotnet.instance.Module.canvas = canvas;
    await dotnet.run();
});
//# sourceMappingURL=index.js.map