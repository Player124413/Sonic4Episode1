#if BGM_STREAM
using System;
using System.IO;
using NVorbis;

internal partial class AppMain
{
    internal sealed class CriAuBgmStream : IDisposable
    {
        private VorbisReader m_intro;
        private VorbisReader m_loop;
        private VorbisReader m_active;
        private bool m_inLoop;

        public CriAuBgmStream(Stream intro, Stream loop)
        {
            m_intro = new VorbisReader(intro, true);
            m_loop = loop != null ? new VorbisReader(loop, true) : null;
            m_active = m_intro;
        }

        public int SampleRate => m_intro.SampleRate;

        public int Channels => m_intro.Channels;

        public long IntroSamples => m_intro.TotalSamples;

        public long LoopSamples => m_loop != null ? m_loop.TotalSamples : 0L;

        public bool InLoop => m_inLoop;

        public int Read(float[] buffer, int count)
        {
            count -= count % Channels;

            int total = 0;
            while (total < count)
            {
                var read = m_active.ReadSamples(buffer, total, count - total);
                if (read > 0)
                {
                    total += read;
                    continue;
                }

                if (!Advance())
                    break;
            }

            return total;
        }

        public void Rewind()
        {
            m_active = m_intro;
            m_inLoop = false;
            Restart(m_intro);
            Restart(m_loop);
        }

        public void Dispose()
        {
            m_intro?.Dispose();
            m_loop?.Dispose();
            m_intro = m_loop = m_active = null;
        }

        private bool Advance()
        {
            if (m_loop == null)
                return false;

            if (!m_inLoop)
            {
                m_active = m_loop;
                m_inLoop = true;
            }

            Restart(m_active);
            return true;
        }

        // seeking a reader that has never decoded a packet walks an empty page list and throws
        private static void Restart(VorbisReader reader)
        {
            if (reader != null && reader.SamplePosition > 0L)
                reader.SeekTo(0L, SeekOrigin.Begin);
        }
    }
}
#endif
