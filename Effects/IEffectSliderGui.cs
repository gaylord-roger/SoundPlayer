using System;

namespace NexusSpeech.Effects
{
    interface IEffectSliderGui
    {
        void Initialize(Slider slider);
        event EventHandler ValueChanged;
    }
}
