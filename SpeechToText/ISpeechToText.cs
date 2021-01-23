using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusSpeech.SpeechToText
{
    public interface ISpeechToText
    {
        Action Start(Action<string> speak, Action onStopped);
    }
}
