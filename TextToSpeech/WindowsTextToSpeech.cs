using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace NexusSpeech.TextToSpeech
{
    public class WindowsTextToSpeech : ISynthesizer
    {
        public InstalledVoice Voice { get; set; }

        public async Task Speak(string text, BufferedWaveProvider waveProvider, int rate)
        {
            var fmt = new System.Speech.AudioFormat.SpeechAudioFormatInfo(waveProvider.WaveFormat.SampleRate, (System.Speech.AudioFormat.AudioBitsPerSample)waveProvider.WaveFormat.BitsPerSample, (System.Speech.AudioFormat.AudioChannel)waveProvider.WaveFormat.Channels);

            var _synthesizer = new SpeechSynthesizer()
            {
                Volume = 100,  // 0...100
                Rate = rate,   // -10...10
            };
            _synthesizer.SelectVoice(Voice.VoiceInfo.Name);
            _synthesizer.SetOutputToAudioStream(new OutputStream(waveProvider), fmt);

            _synthesizer.SpeakAsync(text);

            await Task.FromResult(0);
        }

        public override string ToString()
        {
            return Voice.VoiceInfo.Description;
        }
    }

    public class OutputStream : System.IO.Stream
    {
        private BufferedWaveProvider provider;

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => throw new NotImplementedException();

        public override long Position { get => 0; set => throw new NotImplementedException(); }

        public OutputStream(BufferedWaveProvider provider)
        {
            this.provider = provider;
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            provider.AddSamples(buffer, offset, count);
        }
    }
}
