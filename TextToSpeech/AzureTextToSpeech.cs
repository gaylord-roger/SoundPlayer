using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusSpeech.TextToSpeech
{
    public class AzureTextToSpeech : ISynthesizer
    {
        public string Key { get; set; }
        public string Region { get; set; }
        public string Language { get; set; }
        public string Voice { get; set; }
        public Action<string> OnLog { get; set; }

        public async Task Speak(string text, BufferedWaveProvider waveProvider, int rate)
        {
            var fmt = new System.Speech.AudioFormat.SpeechAudioFormatInfo(waveProvider.WaveFormat.SampleRate, (System.Speech.AudioFormat.AudioBitsPerSample)waveProvider.WaveFormat.BitsPerSample, (System.Speech.AudioFormat.AudioChannel)waveProvider.WaveFormat.Channels);

            // Creates an instance of a speech config with specified subscription key and service region.
            // Replace with your own subscription key and service region (e.g., "westus").
            var config = SpeechConfig.FromSubscription(Key, Region);
            config.SpeechSynthesisLanguage = Language;
            config.SpeechSynthesisVoiceName = Voice;
            
            // Creates an audio out stream.
            using (var stream = AudioOutputStream.CreatePullStream(AudioStreamFormat.GetWaveFormatPCM((uint)waveProvider.WaveFormat.SampleRate, (byte)waveProvider.WaveFormat.BitsPerSample, (byte)waveProvider.WaveFormat.Channels)))
            {
                // Creates a speech synthesizer using audio stream output.
                using (var streamConfig = AudioConfig.FromStreamOutput(stream))
                using (var synthesizer = new SpeechSynthesizer(config, streamConfig))
                {
                    using (var result = await synthesizer.SpeakTextAsync(text))
                    {
                        if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                        {
                            //Console.WriteLine($"Speech synthesized for text [{text}], and the audio was written to output stream.");
                        }
                        else if (result.Reason == ResultReason.Canceled)
                        {
                            var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                            OnLog?.Invoke($"CANCELED: Reason={cancellation.Reason}");

                            if (cancellation.Reason == CancellationReason.Error)
                            {
                                OnLog?.Invoke($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                                OnLog?.Invoke($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                                OnLog?.Invoke($"CANCELED: Did you update the subscription info?");
                            }
                        }
                    }
                }
/*
                using (var reader = new WaveFileReader(new PullStream(stream)))
                {
                    var newFormat = new WaveFormat(waveProvider.WaveFormat.SampleRate, waveProvider.WaveFormat.BitsPerSample, waveProvider.WaveFormat.Channels);
                    using (var conversionStream = new WaveFormatConversionStream(newFormat, reader))
                    {
                        //WaveFileWriter.CreateWaveFile("output.wav", conversionStream);
                        byte[] buffer = new byte[32000];
                        int filledSize = 0;
                        int totalSize = 0;
                        while ((filledSize = conversionStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            waveProvider.AddSamples(buffer, 0, (int)filledSize);
                            //Console.WriteLine($"{filledSize} bytes received.");
                            totalSize += filledSize;
                        }
                    }
                }*/

                
                // Reads(pulls) data from the stream
                byte[] buffer = new byte[32000];
                uint filledSize = 0;
                uint totalSize = 0;
                while ((filledSize = stream.Read(buffer)) > 0)
                {
                    waveProvider.AddSamples(buffer, 0, (int)filledSize);
                    //Console.WriteLine($"{filledSize} bytes received.");
                    totalSize += filledSize;
                }
            }
        }

        public override string ToString()
        {
            return "Azure " + Voice;
        }
    }

    public class PullStream : Stream
    {
        private PullAudioOutputStream _stream;

        public PullStream(PullAudioOutputStream stream)
        {
            _stream = stream;
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => 0;

        public override long Position { get => 0; set => throw new NotImplementedException(); }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return (int)_stream.Read(buffer);
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
            throw new NotImplementedException();
        }
    }
}
