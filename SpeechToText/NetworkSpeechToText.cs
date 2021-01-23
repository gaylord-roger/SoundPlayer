using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NexusSpeech.SpeechToText
{
    public class NetworkSpeechToText : ISpeechToText
    {
        public int Port { get; set; }

        public Action Start(Action<string> speak, Action onStopped)
        {
            var udpServer = new UdpClient(Port);
            var thread = new Thread(new ParameterizedThreadStart(StartRecognizing1));
            thread.Start(new Data() { onStopped = onStopped, speak = speak, server = udpServer });

            return () =>
            {
                if (udpServer != null)
                {
                    udpServer.Close();
                }
            };
        }

        public class Data
        {
            public Action<string> speak;
            public Action onStopped;
            public UdpClient server;
        }

        void StartRecognizing1(object obj)
        {
            Data dat = (Data)obj;
            try
            {
                RecognizeSpeechAsync(dat.speak, dat.server).GetAwaiter().GetResult();
            }
            catch { }
            dat.onStopped?.Invoke();
        }

        private async Task RecognizeSpeechAsync(Action<string> speak, UdpClient server)
        {
            while (server != null)
            {
                try
                {
                    //IPEndPoint object will allow us to read datagrams sent from any source.
                    var msg = await server.ReceiveAsync();
                    var txt = System.Text.UTF8Encoding.UTF8.GetString(msg.Buffer);
                    speak(txt);
                }
                catch
                {
                    server = null;
                }
            }
        }

        public override string ToString()
        {
            return "Network socket on port " + Port;
        }
    }
}
