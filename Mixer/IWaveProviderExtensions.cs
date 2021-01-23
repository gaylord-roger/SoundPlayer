using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusSpeech
{
    public static class IWaveProviderExtensions
    {
        public static IWaveProvider ConvertToRightChannelCount(this IWaveProvider input, WaveFormat format)
        {
            if (input.WaveFormat.Channels == format.Channels)
            {
                return input;
            }
            if (input.WaveFormat.Channels == 1 && format.Channels == 2)
            {
                return new MonoToStereoProvider16(input);
            }/*
            if (input.WaveFormat.Channels == 2 && format.Channels == 1)
            {
                return new StereoToMonoProvider16(input);
            }
            throw new NotImplementedException($"Not yet implemented this channel count conversion: {input.WaveFormat.Channels} -> {format.Channels}");
            */
            return input;
        }

        public static IWaveProvider ConvertToRightSampleRate(this IWaveProvider input, WaveFormat format)
        {
            if (input.WaveFormat.SampleRate == format.SampleRate)
            {
                return input;
            }
            return new WdlResamplingSampleProvider(input.ToSampleProvider(), format.SampleRate).ToWaveProvider();
        }

        public static IWaveProvider ConvertToRightWaveFormat(this IWaveProvider input, WaveFormat format)
        {
            return input
                    .ConvertToRightChannelCount(format)
                    .ConvertToRightSampleRate(format);
        }
    }
}
