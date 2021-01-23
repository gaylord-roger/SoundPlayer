using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NexusSpeech.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusSpeech
{
    public class PlaybackMixer
    {
        private MixingSampleProvider mixer;
        private Pcm16BitToSampleProvider bufferStream32;
        private EffectStream effectStream;
        private WasapiOut output;

        public bool IsPlaying { get; private set; }

        public PlaybackMixer(WaveFormat waveFormat)
        {
            /*
            var mmFmt = outputDevice.AudioClient.MixFormat;
            WaveFormat = new WaveFormat(mmFmt.SampleRate, mmFmt.BitsPerSample <= 16 ? mmFmt.BitsPerSample : 16, mmFmt.Channels);
            */
            WaveFormat = waveFormat;

            mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(WaveFormat.SampleRate, WaveFormat.Channels));
            mixer.ReadFully = true;

            // convert to 32 bit floating point
            bufferStream32 = new Pcm16BitToSampleProvider(mixer.ToWaveProvider16());
            // pass through the effects
            effectStream = new EffectStream(bufferStream32);
            //effectStream.UpdateEffectChain(effects.ToArray());
        }

        public static MMDevice[] GetInputDevices()
        {
            var enumerator = new MMDeviceEnumerator();
            return enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).ToArray();
        }

        public static MMDevice[] GetOutputDevices()
        {
            var enumerator = new MMDeviceEnumerator();
            return enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active).ToArray();
        }


        public void Start(MMDevice outputDevice)
        {
            output = new WasapiOut(outputDevice, AudioClientShareMode.Shared, true, 0);
            output.Init(effectStream);
            output.Play();

            IsPlaying = true;
        }

        public void Stop()
        {
            output.Stop();
            IsPlaying = false;
        }

        public void UpdateEffectChain(Effect[] newEffects)
        {
            effectStream.UpdateEffectChain(newEffects);
        }

        public PlaybackTrack AddPlaybackTrack()
        {
            return new PlaybackTrack(mixer);            
        }

        public WaveFormat WaveFormat { get; private set; }
    }
}
