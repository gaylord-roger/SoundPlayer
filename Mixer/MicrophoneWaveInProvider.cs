using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusSpeech
{
    public class MicrophoneWaveInProvider : WaveInProvider
    {
        private readonly WasapiCapture source;

        public MicrophoneWaveInProvider(WasapiCapture source) : base(source)
        {
            this.source = source;
            source.StartRecording();
        }

        public void Stop()
        {
            source.StopRecording();
        }

        public static MicrophoneWaveInProvider CreateMicrophone(MMDevice device, WaveFormat waveFormat, int audioBufferMillisecondsLength = 25)
        {
            var capture = new WasapiCapture(device, true, audioBufferMillisecondsLength) { WaveFormat = waveFormat };
            
            return new MicrophoneWaveInProvider(capture);
        }
    }
}
