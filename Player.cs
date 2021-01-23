using NAudio.Wave;
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
    public class Player
    {
        public ISpeechToText Source { get; private set; }
        public ISynthesizer Synthesizer { get; private set; }
        public int Rate { get; private set; }
        public Control CtrlActive { get; private set; }
        public BufferedWaveProvider WaveProvider { get; private set; }

        public Action<string> Log { get; set; }
        public Action OnStart { get; set; }
        public Action OnStop { get; set; }

        public Player(PlayerConfig config, BufferedWaveProvider waveProvider)
        {
            Source = config.GetSpeechToText();
            Synthesizer = config.GetSynthesizer();
            Rate = config.GetVoiceRate();
            CtrlActive = config.GetActivityControl();
            WaveProvider = waveProvider;
        }

        protected virtual void Starting()
        {
            OnStart?.Invoke();
            SetCtrlVisibility(CtrlActive, true);
        }

        protected virtual void Stopped()
        {
            OnStop?.Invoke();
            SetCtrlVisibility(CtrlActive, false);
        }

        public bool CheckParameters()
        {
            if (Source == null || Synthesizer == null)
            {
                MessageBox.Show("Please select source and synthesizer");
                return false;
            }
            if (Rate < -10 || Rate > 10)
            {
                MessageBox.Show("Invalid voice rate");
            }
            return true;
        }

        /// <summary>
        /// Allow to cancel the action
        /// </summary>
        private Action cancel;
        public void Start()
        {
            if (!CheckParameters()) return;

            Starting();
            
            try
            {
                cancel = Source.Start(
                    async (text) =>
                    {
                        Log(text);
                        await Synthesizer.Speak(text, WaveProvider, Rate);
                    },
                    () =>
                    {
                        cancel = null;
                        Stopped();
                    }
                );
            }
            catch { }
        }

        public void Cancel()
        {
            cancel?.Invoke();
        }

        private delegate void SafeCallDelegate(Control ctrl, bool b);
        void SetCtrlVisibility(Control ctrl, bool b)
        {
            if (ctrl.InvokeRequired)
            {
                var d = new SafeCallDelegate(SetCtrlVisibility);
                ctrl.Invoke(d, new object[] { ctrl, b });
            }
            else
            {
                ctrl.Visible = b;
            }
        }
    }
}
