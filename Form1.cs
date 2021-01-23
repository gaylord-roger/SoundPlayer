using NAudio.CoreAudioApi;
using NAudio.Wave;
using NexusSpeech.Effects;
using System;
using System.Linq;
using System.Windows.Forms;

namespace NexusSpeech
{
    public partial class Form1 : Form
    {
        KeyboardHook hook = new KeyboardHook();

        private EffectChain effects;

        private bool _muteInputMic;

        private PlaybackMixer playbackMixer;
        private MicrophoneWaveInProvider microphone;
        private PlaybackTrack trackMicrophone;
        private PlaybackTrack trackWaves;

        public Form1()
        {
            InitializeComponent();

            // register the event that is fired after the key press.
            hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(hook_KeyPressed);
            hook.RegisterHotKey(NexusSpeech.ModifierKeys.Control, Keys.F1);
            hook.RegisterHotKey(NexusSpeech.ModifierKeys.Control, Keys.F2);

            effects = new EffectChain();
            effects.Modified += (s, a) => playbackMixer?.UpdateEffectChain(effects.ToArray());
        }

        void hook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            /*
            var instance = configs.Select((p, index) => new { index, p }).FirstOrDefault(item => item.p.Modifier == e.Modifier && item.p.Key == e.Key);

            var player = players[instance.index];
            if (player != null)
            {
                player.Cancel();
                players[instance.index] = null;
                return;
            }
            player = CreatePlayer(instance.index);
            player.Start();*/
        }

        private delegate void LogDelegate(string text);
        private void Log(string text)
        {
            if (richTextBox1.InvokeRequired)
            {
                var dlg = new LogDelegate(Log);
                richTextBox1.Invoke(dlg, new[] { text });
            }
            else
            {
                richTextBox1.AppendText(text + "\r\n");
            }
        }
        
        private void StartOuput(MMDevice input, MMDevice output)
        {
            Stop();

            if (output == null)
            {
                MessageBox.Show("No output ?");
            }

            // MessageBox.Show(output.AudioClient.MixFormat.SampleRate + "/" + output.AudioClient.MixFormat.Channels);

            var wf = new WaveFormat(44100, 16, 2);

            try
            {
                playbackMixer = new PlaybackMixer(wf);
                playbackMixer.Start(output);

                trackMicrophone = playbackMixer.AddPlaybackTrack();
                trackWaves = playbackMixer.AddPlaybackTrack();


                if (input != null)
                {
                    microphone = MicrophoneWaveInProvider.CreateMicrophone(input, input.AudioClient.MixFormat);
                    trackMicrophone.Play(microphone);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void Stop()
        {
            if (playbackMixer != null)
            {
                playbackMixer.Stop();
                playbackMixer = null;

                microphone?.Stop();
            }
        }

        private void StartStop()
        {
            try
            {
                var active = playbackMixer == null;
                comboBoxOutputDevices.Enabled = !active;

                if (active)
                {
                    button1.Text = "Stop";
                    StartOuput((MMDevice)comboBoxInputDevices.SelectedItem, (MMDevice)comboBoxOutputDevices.SelectedItem);
                }
                else
                {
                    button1.Text = "Start";
                    Stop();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StartStop();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            inputDeviceBindingSource.DataSource = PlaybackMixer.GetInputDevices();
            mMDeviceBindingSource.DataSource = PlaybackMixer.GetOutputDevices();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Stop();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SelectVoice(Convert.ToInt32(((Button)sender).Tag));
        }

        int _voice = 0;
        void SelectVoice(int index)
        {
            if (index == _voice)
                return;

            _voice = index;
            Effect effect = null;
            switch (_voice)
            {
                case 1:
                    effect = new Chorus();
                    break;
                case 2:
                    effect = new FastAttackCompressor1175();
                    break;
                case 3:
                    effect = new BadBussMojo();
                    break;
                case 4:
                    effect = new EventHorizon();
                    break;
                case 5:
                    effect = new FairlyChildish();
                    break;
                case 6:
                    effect = new FlangeBaby();
                    break;
                case 7:
                    effect = new SuperPitch();
                    break;
                case 8:
                    effect = new ThreeBandEQ();
                    break;
                case 9:
                    effect = new Tremolo();
                    break;
                default:
                    break;
            }

            if (effects.Count > 0)
            {
                effects.Clear();
            }
            if (effect != null)
            {
                effects.Add(effect);
            }

            panel1.Controls.Clear();
            if (effect != null)
            {
                var panel = new EffectPanel();
                panel.Dock = DockStyle.Fill;
                panel.Initialize(effect);
                panel1.Controls.Add(panel);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            _muteInputMic = !checkBox1.Checked;
        }

        private void button12_Click(object sender, EventArgs e)
        {
        }

        private void Play(string filename)
        {
            trackWaves.Play(new Mp3FileReader(filename));
        }

        private void button12_Click_1(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog()
            {
                Title = "Select sound file"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Play(ofd.FileName);
            }
        }
    }


    class AutoDisposeFileReader : ISampleProvider
    {
        private readonly AudioFileReader reader;
        private bool isDisposed;
        public AutoDisposeFileReader(AudioFileReader reader)
        {
            this.reader = reader;
            this.WaveFormat = reader.WaveFormat;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            if (isDisposed)
                return 0;
            int read = reader.Read(buffer, offset, count);
            if (read == 0)
            {
                reader.Dispose();
                isDisposed = true;
            }
            return read;
        }

        public WaveFormat WaveFormat { get; private set; }
    }
}