// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

import install, { Progress } from './installer.js';

import Accelerometer from './accelerometer.js';
import Guide from './guide.js';
import WasmMediaPlayer from './mediaplayer.js';
// @ts-ignore
import { dotnet } from '../_framework/dotnet.js';

const loader = document.querySelector("#uno-body")! as HTMLElement;
const progress = document.querySelector('#progress')! as HTMLProgressElement;
const status = document.querySelector('#status')! as HTMLSpanElement;
const canvas = document.querySelector('#canvas')! as HTMLCanvasElement;

const tapToPlay = document.querySelector('#tap-to-play')! as HTMLButtonElement;

const updateProgress = (p: Progress) => {
    status.textContent = p.status;
    progress.value = (p.progress / 3) + (p.step! / 3);
}

const resize = () => {
    const width = window.innerWidth;
    const height = window.innerHeight;

    const scale = Math.min(width / 800, height / 480);
    canvas.style.width = `${800 * scale}px`;
    canvas.style.height = `${480 * scale}px`;
    canvas.style.marginTop = `${(height - 480 * scale) / 2}px`;
}

const onInstall = () => {
    if ('umami' in window) {
        (window.umami as any)?.track('App installed');
    }
}

window.onresize = resize;
window.addEventListener('appinstalled', onInstall);

const { setModuleImports, getAssemblyExports, getConfig } = await dotnet
    .withDiagnosticTracing(false)
    .withApplicationArguments(window.location.href)
    .create();

const FS = dotnet.instance.Module.FS;
const IDBFS = dotnet.instance.Module.IDBFS;

const S4E1 = await getAssemblyExports("Sonic4Episode1.Wasm.dll")
const mediaPlayer = new WasmMediaPlayer(S4E1);

setModuleImports('main.js', {
    setMainLoop: (cb: any) => dotnet.instance.Module.setMainLoop(cb),
    go: () => {
        loader.style.display = "none";
        tapToPlay.style.display = "none";
        resize();

        canvas.focus();
    },

    syncFS: () => new Promise<void>((res, rej) => {
        FS.syncfs(false, (err: any) => {
            if (err) rej(err);

            res();
        });
    }),

    reportProgress: (str: string, prog: number) => {
        status.textContent = str;
        progress.value = prog;
    },

    MediaPlayer_GetVolume: mediaPlayer.getVolume.bind(mediaPlayer),
    MediaPlayer_SetVolume: mediaPlayer.setVolume.bind(mediaPlayer),
    MediaPlayer_Play: (song: string) => mediaPlayer.play(song),
    MediaPlayer_PlayLooped: (song: string, loop: number) => mediaPlayer.play(song, loop),
    MediaPlayer_LoadSong: (song: string) => mediaPlayer.preload(song),
    MediaPlayer_Stop: mediaPlayer.stop.bind(mediaPlayer),

    XmlStorage_GetSaveFile: () => localStorage.getItem('saveData'),
    XmlStorage_SetSaveFile: (data: string) => localStorage.setItem('saveData', data),

    SaveState_GetSaveState: () => localStorage.getItem('saveState'),
    SaveState_SetSaveState: (data: string) => localStorage.setItem('saveState', data),
    SaveState_HasSaveState: () => localStorage.getItem('saveState') != null,
    SaveState_DeleteSaveState: () => localStorage.removeItem('saveState'),

    Guide_BeginShowMessageBox: Guide.beginShowMessageBox.bind(Guide),

    WebBrowserTask_Open: (url: string) => window.open(url, '_blank'),
});

FS.mkdir('/content');
FS.mount(IDBFS, {}, '/content');

FS.syncfs(true, async () => {
    dotnet.instance.Module.canvas = canvas;
    await dotnet.run();
});
