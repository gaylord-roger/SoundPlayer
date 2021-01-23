using NexusSpeech.SpeechToText;
using NexusSpeech.TextToSpeech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NexusSpeech
{
    public class PlayerConfig
    {
        public ModifierKeys Modifier { get; set; }
        public Keys Key { get; set; }
        public Func<ISpeechToText> GetSpeechToText { get; set; }
        public Func<ISynthesizer> GetSynthesizer { get; set; }
        public Func<int> GetVoiceRate { get; set; }
        public Func<Control> GetActivityControl { get; set; }
    }
}
