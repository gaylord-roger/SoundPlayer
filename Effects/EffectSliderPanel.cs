﻿using System;
using System.Windows.Forms;

namespace NexusSpeech.Effects
{
    public partial class EffectSliderPanel : UserControl, IEffectSliderGui
    {
        Slider slider;

        public EffectSliderPanel()
        {
            InitializeComponent();
            this.metroTrackBar1.Minimum = 0;
            this.metroTrackBar1.Maximum = 1000;
            this.metroTrackBar1.SmallChange = 50;
            this.metroTrackBar1.LargeChange = 100;
            //this.metroTrackBar1.TickFrequency = 50;
            this.metroTrackBar1.Scroll += OnTrackbackScroll;            
        }

        void OnTrackbackScroll(object sender, EventArgs e)
        {
            if (this.slider != null)
            {
                float value = TrackBarToSlider();
                this.slider.Value = value;
                SetSliderTextBox(value);
                // put it back to show granularity
                var trackBarVal = SliderToTrackBar(slider.Minimum, slider.Maximum);
                metroTrackBar1.Value = trackBarVal;
                RaiseValueChangedEvent(e);
            }
        }

        private void SetSliderTextBox(float value)
        {
            if (slider.DiscreteValueText.Count > 0)
            {
                int index = (int)slider.Value;
                textBoxValue.Text = slider.DiscreteValueText[index];
            }
            else
            {
                textBoxValue.Text = value.ToString();
            }
        }

        private void RaiseValueChangedEvent(EventArgs e)
        {
            if (this.ValueChanged != null)
            {
                this.ValueChanged(this, e);
            }
        }

        float TrackBarToSlider()
        {
            float value = 0;

            if (metroTrackBar1.Value == metroTrackBar1.Maximum)
            {
                value = slider.Maximum;
            }
            else
            {
                value = slider.Minimum + (this.metroTrackBar1.Value * (slider.Maximum - slider.Minimum) / 1000.0f);
                value -= value % slider.Increment;
            }

            return value;
        }

        int SliderToTrackBar(float min, float max)
        {
            var val = (int) (((slider.Value - min) / (max - min)) * 1000.0f);
            if (val < min) val = (int)min;
            return val;
        }

        public void Initialize(Slider slider)
        {
            this.slider = slider;
            this.metroTrackBar1.Value = SliderToTrackBar(slider.Minimum, slider.Maximum);
            this.labelDescription.Text = slider.Description;
            this.metroTrackBar1.LargeChange = (int)(metroTrackBar1.Maximum * (slider.Increment / (slider.Maximum - slider.Minimum)));
            SetSliderTextBox(slider.Value);
        }

        public event EventHandler ValueChanged;
    }
}
