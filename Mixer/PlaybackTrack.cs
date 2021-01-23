using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusSpeech
{
    public class PlaybackTrack : ISampleProvider
    {
        private readonly MeteringSampleProvider meter;
        private readonly MixingSampleProvider parentMixer;
        private readonly MixingSampleProvider mixer;

        public PlaybackTrack(MixingSampleProvider parent)
        {
            parentMixer = parent;
            mixer = new MixingSampleProvider(parent.WaveFormat) { ReadFully = true };
            meter = new MeteringSampleProvider(mixer);

            parentMixer.AddMixerInput(meter);
        }

        public WaveFormat WaveFormat => meter.WaveFormat;
        public int Read(float[] buffer, int offset, int count) => meter.Read(buffer, offset, count);

        public void Play(IWaveProvider source)
        {
            var provider = source.ConvertToRightWaveFormat(mixer.WaveFormat);

            mixer.AddMixerInput(provider);
        }
    }
}
