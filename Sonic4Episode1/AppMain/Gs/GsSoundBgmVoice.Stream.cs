#if BGM_STREAM
using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

internal partial class AppMain
{
    internal sealed class CriAuBgmVoice
    {
        public const bool SupportsFade = true;

        private const int BufferMilliseconds = 100;
        private const int TargetPendingBuffers = 8;
        private const int PrimePendingBuffers = 3;
        private const int BuffersPerUpdate = 2;

        private CriAuBgmStream m_stream;
        private DynamicSoundEffectInstance m_output;
        private float[] m_scratch;
        private byte[] m_scratchBytes;
        private int m_bufferSamples;
        private int m_channels;
        private long m_decodedSamples;
        private long m_playedSamples;
        private bool m_draining;
        private bool m_paused;
        private bool m_ended;
        private float m_volume = 1f;

        public bool Ended => m_ended;

        public float Volume
        {
            get => m_volume;
            set
            {
                m_volume = value;
                if (m_output != null && !m_output.IsDisposed)
                    m_output.Volume = value;
            }
        }

        public uint NumPlayedSamples
        {
            get
            {
                if (m_output == null || m_output.IsDisposed)
                    return (uint)m_playedSamples;

                var queued = (long)m_output.PendingBufferCount * m_bufferSamples;
                var estimate = m_decodedSamples - queued;
                if (estimate > m_playedSamples)
                    m_playedSamples = estimate;

                return (uint)m_playedSamples;
            }
        }

        public static void SystemInit()
        {
        }

        public static bool Preload(string track, bool loop)
        {
            return CriAuBgmSource.Prepare(track, loop);
        }

        public static void ClearPreload()
        {
            CriAuBgmSource.Clear();
        }

        public bool Load(string cue, string track, bool loop)
        {
            Release();

            try
            {
                m_stream = new CriAuBgmStream(
                    CriAuBgmSource.Open(track, CriAuBgmSource.Intro),
                    loop ? CriAuBgmSource.Open(track, CriAuBgmSource.Loop) : null);
            }
            catch (Exception ex)
            {
                ex.ToString();
                Release();
                return false;
            }

            m_channels = m_stream.Channels;
            m_bufferSamples = m_stream.SampleRate * BufferMilliseconds / 1000;
            m_scratch = new float[m_bufferSamples * m_channels];
            m_scratchBytes = new byte[m_scratch.Length * 2];
            m_output = new DynamicSoundEffectInstance(m_stream.SampleRate, (AudioChannels)m_channels);
            m_output.Volume = m_volume;
            return true;
        }

        public void Play()
        {
            if (m_output == null)
                return;

            for (int i = 0; i < PrimePendingBuffers; ++i)
            {
                if (!SubmitOne())
                    break;
            }

            m_output.Play();
            m_paused = false;
        }

        public void Stop()
        {
            m_paused = false;
            m_draining = false;
            m_ended = false;

            if (m_output != null && !m_output.IsDisposed)
                m_output.Stop();

            m_stream?.Rewind();
            m_decodedSamples = 0;
            m_playedSamples = 0;
        }

        public void Pause(bool pause)
        {
            if (m_output == null || m_output.IsDisposed)
                return;

            if (pause)
            {
                m_output.Pause();
                m_paused = true;
            }
            else
            {
                m_output.Resume();
                m_paused = false;
            }
        }

        public void Update()
        {
            if (m_output == null || m_output.IsDisposed || m_ended)
                return;

            if (m_paused || m_output.State != SoundState.Playing)
                return;

            if (m_draining)
            {
                if (m_output.PendingBufferCount == 0)
                {
                    m_output.Stop();
                    m_ended = true;
                }

                return;
            }

            for (int i = 0; i < BuffersPerUpdate && m_output.PendingBufferCount < TargetPendingBuffers; ++i)
            {
                if (!SubmitOne())
                    break;
            }
        }

        public void Release()
        {
            if (m_output != null)
            {
                m_output.Stop();
                m_output.Dispose();
                m_output = null;
            }

            m_stream?.Dispose();
            m_stream = null;
            m_scratch = null;
            m_bufferSamples = 0;
            m_channels = 0;
            m_decodedSamples = 0;
            m_playedSamples = 0;
            m_draining = false;
            m_paused = false;
            m_ended = false;
        }

        private bool SubmitOne()
        {
            var read = m_stream.Read(m_scratch, m_scratch.Length);
            if (read == 0)
            {
                m_draining = true;
                return false;
            }

#if FNA || MONOGAME
            m_output.SubmitFloatBufferEXT(m_scratch, 0, read);
#else
            var buffer = m_scratchBytes;
            var floatBuffer = m_scratch;
            for (int i = 0; i < read; i++)
            {
                short sValue = (short)(short.MaxValue * floatBuffer[i]);
                buffer[i * 2] = (byte)(sValue & 0xff);
                buffer[i * 2 + 1] = (byte)((sValue >> 8) & 0xff);
            }

            m_output.SubmitBuffer(buffer, 0, read * 2);
#endif
            m_decodedSamples += read / m_channels;
            return true;
        }
    }
}
#endif
