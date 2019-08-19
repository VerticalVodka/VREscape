using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace VREscape
{
    public class RadioRotary : Rotary
    {
        public bool IsRadioOn = false;
        public IDictionary<int, AudioClip> Frequencies = new Dictionary<int, AudioClip>();
        public AudioSource ChannelAudioSource;
        private bool _channelIsPlaying = false;
        public AudioSource BackGroundAudioSource;

        public int[] RoundToNumbers = new int[] { 800, 927, 1000, 1104 };

        public TextMeshPro CurrentFrequency;

        public RadioRotary()
        {
            changeMultiplier = -18;
        }

        public override void Update()
        {
            base.Update();
            if (!IsRadioOn)
            {
                CurrentFrequency.SetText("");
                BackGroundAudioSource.Stop();
                ChannelAudioSource.Stop();
            }
            else
            {
                int roundedFreq = RoundedFrequenceyIfNeeded();
                CurrentFrequency.SetText($"{(roundedFreq / 10d):0.0}");
                if (!BackGroundAudioSource.isPlaying)
                    BackGroundAudioSource.Play();
            }

            for (var i = -5; i <= 5; ++i)
            {
                int roundedState = RoundedFrequenceyIfNeeded();
                if (Frequencies.ContainsKey(roundedState + i))
                {
                    ChannelAudioSource.volume = (1.0f - Mathf.Abs((float)i) / 5.0f) * .3f;
                    BackGroundAudioSource.volume = .2f * Mathf.Abs((float) i) / 5.0f;
                    if (!_channelIsPlaying)
                    {
                        ChannelAudioSource.clip = Frequencies[roundedState + i];
                        ChannelAudioSource.Play();
                        _channelIsPlaying = true;
                    }

                    _channelIsPlaying = ChannelAudioSource.isPlaying;
                }
            }
        }

        private int RoundedFrequenceyIfNeeded()
        {
            for(int i = 0; i < RoundToNumbers.Length; ++i)
            {
                if (Math.Abs(CurrentState - RoundToNumbers[i]) <= 4)
                {
                    Debug.Log($"Rounding {CurrentState} to {RoundToNumbers[i]}");
                    return RoundToNumbers[i];
                }
            }
            return CurrentState;
        }
    }
}