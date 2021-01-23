using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;

namespace NexusSpeech.SpeechToText
{
    class WindowsSpeechToText : ISpeechToText
    {
        public Action Start(Action<string> speak, Action onStopped)
        {
            var _recognition = new SpeechRecognitionEngine();
            DictationGrammar dg = new DictationGrammar();
            _recognition.LoadGrammar(dg);
            _recognition.SpeechRecognized += (a, b) =>
            {
                var txt = b.Result.Text;
                speak(txt);
            };
            _recognition.SetInputToDefaultAudioDevice();
            _recognition.RecognizeAsync(RecognizeMode.Multiple);

            return () =>
            {
                _recognition.RecognizeAsyncCancel();
                _recognition.RecognizeCompleted += (a, b) =>
                {
                    _recognition.SetInputToNull();
                };
            };
        }

        public override string ToString()
        {
            return "Windows TTS from default communication device";
        }
    }
}
