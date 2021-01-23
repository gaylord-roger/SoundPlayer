using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusSpeech.TextToSpeech
{
    public interface ISynthesizer
    {
        Task Speak(string text, BufferedWaveProvider waveProvider, int rate);
    }
}
