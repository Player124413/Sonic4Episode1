#if BGM_MEDIAPLAYER
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Media;

internal partial class AppMain
{
    internal sealed class CriAuBgmVoice
    {
#if WASM
        public const bool SupportsFade = false;
#else
        public const bool SupportsFade = true;
#endif

        public static Song m_songBGMIntro;
        public static Song m_songBGMLoop;
        public static string ActiveCue;

        private static readonly Dictionary<string, Song> bgmPreloadedList = new Dictionary<string, Song>();

#if MONOGAME
        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "Add")]
        extern static void MediaQueue_Add(MediaQueue c, Song song);
#endif

        private string m_cue;
        private float m_volume = 1f;

        public bool Ended => false;

        public uint NumPlayedSamples => 1;

        public float Volume
        {
            get => m_volume;
            set
            {
                m_volume = value;
                if (ActiveCue != m_cue)
                    return;

                Microsoft.Xna.Framework.Media.MediaPlayer.Volume = value;
            }
        }

        public static void SystemInit()
        {
            Microsoft.Xna.Framework.Media.MediaPlayer.MediaStateChanged += MediaPlayer_MediaStateChanged;
            Microsoft.Xna.Framework.Media.MediaPlayer.ActiveSongChanged += MediaPlayer_ActiveSongChanged;
        }

        public static bool Preload(string track, bool loop)
        {
            if (loop)
            {
                var intro = Sonic4Ep1.pInstance.Content.Load<Song>($"SOUND\\{track}\\Intro");
                var body = Sonic4Ep1.pInstance.Content.Load<Song>($"SOUND\\{track}\\Loop");
                bgmPreloadedList.Add(track + "Intro", intro);
                bgmPreloadedList.Add(track + "Loop", body);
            }
            else
            {
                var song = Sonic4Ep1.pInstance.Content.Load<Song>("SOUND\\" + track + "\\Intro");
                bgmPreloadedList.Add(track + "Intro", song);
            }

            return true;
        }

        public static void ClearPreload()
        {
            bgmPreloadedList.Clear();
        }

        public bool Load(string cue, string track, bool loop)
        {
            m_cue = cue;

            m_songBGMIntro = GetPreloaded(track + "Intro");
            if (m_songBGMIntro == null)
            {
                m_songBGMIntro = Sonic4Ep1.pInstance.Content.Load<Song>("SOUND\\" + track + "\\Intro");
                bgmPreloadedList.Add(track + "Intro", m_songBGMIntro);

#if WASM
                MediaPlayer.LoadSong(m_songBGMIntro);
#endif
            }

            if (loop)
            {
                m_songBGMLoop = GetPreloaded(track + "Loop");
                if (m_songBGMLoop == null)
                {
                    m_songBGMLoop = Sonic4Ep1.pInstance.Content.Load<Song>("SOUND\\" + track + "\\Loop");
                    bgmPreloadedList.Add(track + "Loop", m_songBGMLoop);
                }
            }
            else
            {
                m_songBGMLoop = null;
            }

            ActiveCue = cue;
            return true;
        }

        public void Play()
        {
#if WASM
            MediaPlayer.Stop();
            MediaPlayer.IsRepeating = false;
            MediaPlayer.Play(m_songBGMIntro, m_songBGMLoop);
#else
            Microsoft.Xna.Framework.Media.MediaPlayer.Stop();
            Microsoft.Xna.Framework.Media.MediaPlayer.IsRepeating = false;
            Microsoft.Xna.Framework.Media.MediaPlayer.Play(m_songBGMIntro);

#if FNA || MONOGAME
            if (m_songBGMLoop != null)
#if FNA
                Microsoft.Xna.Framework.Media.MediaPlayer.LoadSong(m_songBGMLoop);
#elif MONOGAME
                MediaQueue_Add(Microsoft.Xna.Framework.Media.MediaPlayer.Queue, m_songBGMLoop);
#endif
#endif
#endif
        }

        public void Stop()
        {
            if (m_cue != null && m_cue == ActiveCue)
                Microsoft.Xna.Framework.Media.MediaPlayer.Stop();
        }

        public void Pause(bool pause)
        {
            if (pause)
            {
                if (m_cue != ActiveCue)
                    return;

                Microsoft.Xna.Framework.Media.MediaPlayer.Pause();
            }
            else
            {
                Microsoft.Xna.Framework.Media.MediaPlayer.Resume();
            }
        }

        public void Update()
        {
        }

        public void Release()
        {
            m_cue = null;
            if (m_cue != null && m_cue == ActiveCue)
                Microsoft.Xna.Framework.Media.MediaPlayer.Stop();

            ActiveCue = null;
        }

        private static Song GetPreloaded(string name)
        {
            Song song;
            return !bgmPreloadedList.TryGetValue(name, out song) ? null : song;
        }
    }

#if MONOGAME
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "songs")]
    extern static ref List<Song> MediaQueue_songs(MediaQueue c);
#endif

    public static void MediaPlayer_ActiveSongChanged(object sender, EventArgs e)
    {
#if FNA
        Microsoft.Xna.Framework.Media.MediaPlayer.IsRepeating = Microsoft.Xna.Framework.Media.MediaPlayer.Queue.ActiveSongIndex == 1;
#endif
#if MONOGAME
        if (Microsoft.Xna.Framework.Media.MediaPlayer.Queue.ActiveSongIndex == 1)
        {
            Microsoft.Xna.Framework.Media.MediaPlayer.IsRepeating = true;
            MediaQueue_songs(Microsoft.Xna.Framework.Media.MediaPlayer.Queue).RemoveAt(0);
            Microsoft.Xna.Framework.Media.MediaPlayer.Queue.ActiveSongIndex = 0;
        }
#endif
    }

    public static void MediaPlayer_MediaStateChanged(object sender, EventArgs e)
    {
        if (g_ao_sys_global.is_playing_device_bgm_music)
            return;

        MediaState state = Microsoft.Xna.Framework.Media.MediaPlayer.State;
        AMS_CRIAUDIO_INTERFACE global = amCriAudioGetGlobal();
        string str = Microsoft.Xna.Framework.Media.MediaPlayer.Queue.ActiveSong?.Name;
        for (int index = 0; index < global.auply.Length; ++index)
        {
            var auply = global.auply[index];

            if (auply.type == CriAuPlayer.TYPE_BGM && auply.se_name == CriAuBgmVoice.ActiveCue)
            {
                if (state == MediaState.Stopped && auply.status == CriAuPlayer.STATUS_PLAYING)
                {
                    auply.m_stGMState = state;
                    auply.status = CriAuPlayer.STATUS_PLAYEND;
                }
            }

#if !FNA
            if (state != MediaState.Playing &&
                auply.m_stGMState != MediaState.Stopped &&
                auply.m_bLoop && CriAuBgmVoice.m_songBGMLoop != null &&
                str != CriAuBgmVoice.m_songBGMLoop?.Name)
            {
                Microsoft.Xna.Framework.Media.MediaPlayer.Play(CriAuBgmVoice.m_songBGMLoop);
                Microsoft.Xna.Framework.Media.MediaPlayer.IsRepeating = true;
            }
#endif
        }
    }
}
#endif
