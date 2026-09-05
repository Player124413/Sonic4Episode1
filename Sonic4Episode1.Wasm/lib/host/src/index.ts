// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

import Accelerometer from './accelerometer.js';
import Guide from './guide.js';
import { choose, profileFor } from './content.js';
import { install, readBegin, readTake, report } from './store.js';
// @ts-ignore
import { dotnet } from '../_framework/dotnet.js';

const loader = document.querySelector("#uno-body")! as HTMLElement;
const progress = document.querySelector('#progress')! as HTMLProgressElement;
const status = document.querySelector('#status')! as HTMLSpanElement;
const canvas = document.querySelector('#canvas')! as HTMLCanvasElement;


// the game is Hor+ and pillarboxes itself past its own aspect cap (AppMain.mppAspectCap, 2.40), so
// the canvas can just take the whole window on the wide side. nothing caps the narrow side, and a
// tall window crops the level horizontally rather than showing more of it, so letterbox below 3:2
const MIN_ASPECT = 3 / 2;

// past this the render is upscaled into the box instead of following the display. the game is
// authored for 720p and this is a WebGL build with 4x MSAA, so full resolution on a 4K panel is not
// a fight worth picking
const MAX_RENDER_HEIGHT = 1080;

let ready = false;
let viewWidth = 0;
let viewHeight = 0;
let boxWidth = '';
let boxHeight = '';
let boxMargin = '';

// SDL can put its own CSS size on the canvas when the game resizes the window, so the page's box
// gets reasserted each frame rather than set once
const applyBox = () => {
    if (canvas.style.width !== boxWidth)
        canvas.style.width = boxWidth;
    if (canvas.style.height !== boxHeight)
        canvas.style.height = boxHeight;
    if (canvas.style.marginTop !== boxMargin)
        canvas.style.marginTop = boxMargin;
}

const layout = () => {
    const width = window.innerWidth;
    const height = window.innerHeight;

    const boxH = Math.min(height, Math.round(width / MIN_ASPECT));
    boxWidth = `${width}px`;
    boxHeight = `${boxH}px`;
    boxMargin = `${(height - boxH) / 2}px`;
    applyBox();

    if (!ready)
        return;

    const scale = Math.min(window.devicePixelRatio || 1, MAX_RENDER_HEIGHT / boxH);
    const renderWidth = Math.max(1, Math.round(width * scale));
    const renderHeight = Math.max(1, Math.round(boxH * scale));

    if (renderWidth === viewWidth && renderHeight === viewHeight)
        return;

    viewWidth = renderWidth;
    viewHeight = renderHeight;

    canvas.width = renderWidth;
    canvas.height = renderHeight;

    S4E1.Program.SetViewSize(renderWidth, renderHeight);
}

const onInstall = () => {
    if ('umami' in window) {
        (window.umami as any)?.track('App installed');
    }
}

window.onresize = layout;
window.addEventListener('appinstalled', onInstall);

// what to paste back after a playthrough
(window as any).contentReport = () => console.log(report());

// blocks until the player presses Go, so the runtime starts with a content set already decided and
// with the gesture WebAudio wants already spent
const contentProfile = profileFor(await choose());

const { setModuleImports, getAssemblyExports, getConfig } = await dotnet
    .withDiagnosticTracing(false)
    .withApplicationArguments(contentProfile)
    .create();

const FS = dotnet.instance.Module.FS;

const S4E1 = await getAssemblyExports("Sonic4Episode1.Wasm.dll")

setModuleImports('main.js', {
    // setMainLoop: (cb: any) => dotnet.instance.Module.setMainLoop(cb),
    go: (cb: any) => {
        loader.style.display = "none";

        // before the first frame, so the device is created at the size the page wants rather than
        // being made once at the default and reset immediately
        ready = true;
        layout();

        canvas.focus();

        function loop() {
            cb()
            applyBox()
            requestAnimationFrame(loop)
        }
        requestAnimationFrame(loop);
    },

    contentInstall: (profile: string) => install(FS, profile, (str, prog) => {
        status.textContent = str;
        progress.value = prog;
    }),
    contentReadBegin: readBegin,
    contentReadTake: readTake,

    // a content read failed past recovering; the bundles are cached, so starting over is cheap
    reloadPage: () => window.location.reload(),

    XmlStorage_GetSaveFile: () => localStorage.getItem('saveData'),
    XmlStorage_SetSaveFile: (data: string) => localStorage.setItem('saveData', data),

    SaveState_GetSaveState: () => localStorage.getItem('saveState'),
    SaveState_SetSaveState: (data: string) => localStorage.setItem('saveState', data),
    SaveState_HasSaveState: () => localStorage.getItem('saveState') != null,
    SaveState_DeleteSaveState: () => localStorage.removeItem('saveState'),

    Guide_BeginShowMessageBox: Guide.beginShowMessageBox.bind(Guide),

    WebBrowserTask_Open: (url: string) => window.open(url, '_blank'),
});

dotnet.instance.Module.canvas = canvas;
await dotnet.run();
