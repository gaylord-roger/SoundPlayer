using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NexusSpeech.SpeechToText
{
    public class AzureSpeechToText : ISpeechToText
    {
        public string Key { get; set; }
        public string Region { get; set; }
        public string Language { get; set; }
        public Action<string> OnLog { get; set; }

        public Action Start(Action<string> speak, Action onStopped)
        {
            var runner = new ThreadRunner { onStopped = onStopped, Speak = speak, onLog = OnLog, Key = Key, Language = Language, Region = Region };
            var thread = new Thread(runner.Run);
            thread.Start();

            return () =>
            {
            };
        }

        public class ThreadRunner
        {
            public string Key { get; set; }
            public string Region { get; set; }
            public string Language { get; set; }

            public Action<string> Speak;
            public Action onStopped;
            public Action<string> onLog;

            public void Run()
            {
                try
                {
                    RecognizeSpeechAsync().GetAwaiter().GetResult();
                }
                catch { }
                onStopped?.Invoke();
            }

            public async Task RecognizeSpeechAsync()
            {
                // Creates an instance of a speech config with specified subscription key and service region.
                // Replace with your own subscription key // and service region (e.g., "westus").
                var config = Microsoft.CognitiveServices.Speech.SpeechConfig.FromSubscription(Key, Region);
                config.SpeechRecognitionLanguage = Language;
                config.OutputFormat = Microsoft.CognitiveServices.Speech.OutputFormat.Detailed;

                // Creates a speech recognizer.
                using (var recognizer = new Microsoft.CognitiveServices.Speech.SpeechRecognizer(config))
                {
                    //Console.WriteLine("Say something...");

                    // Starts speech recognition, and returns after a single utterance is recognized. The end of a
                    // single utterance is determined by listening for silence at the end or until a maximum of 15
                    // seconds of audio is processed.  The task returns the recognition text as result. 
                    // Note: Since RecognizeOnceAsync() returns only a single utterance, it is suitable only for single
                    // shot recognition like command or query. 
                    // For long-running multi-utterance recognition, use StartContinuousRecognitionAsync() instead.
                    var result = await recognizer.RecognizeOnceAsync();

                    // Checks result.
                    if (result.Reason == Microsoft.CognitiveServices.Speech.ResultReason.RecognizedSpeech)
                    {
                        //Console.WriteLine($"We recognized: {result.Text}");
                        Speak(result.Text);
                    }
                    else if (result.Reason == Microsoft.CognitiveServices.Speech.ResultReason.NoMatch)
                    {
                        onLog?.Invoke($"NOMATCH: Speech could not be recognized.");
                    }
                    else if (result.Reason == Microsoft.CognitiveServices.Speech.ResultReason.Canceled)
                    {
                        var cancellation = Microsoft.CognitiveServices.Speech.CancellationDetails.FromResult(result);
                        onLog?.Invoke($"CANCELED: Reason={cancellation.Reason}");

                        if (cancellation.Reason == Microsoft.CognitiveServices.Speech.CancellationReason.Error)
                        {
                            onLog?.Invoke($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                            onLog?.Invoke($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                            onLog?.Invoke($"CANCELED: Did you update the subscription info?");
                        }
                    }
                }
            }
        }                

        public override string ToString()
        {
            return "Azure speech recognition " + Region + " " + Language;
        }
    }
}
