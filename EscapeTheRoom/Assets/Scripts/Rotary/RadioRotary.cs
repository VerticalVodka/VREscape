using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Collections;

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

        public TextMeshPro CurrentFrequencyTextMesh;

        private int currentlyVisibleFrequency;

        public RadioRotary()
        {
            changeMultiplier = -18;
        }

        public override void Reset()
        {
            base.Reset();
            currentlyVisibleFrequency = CurrentState;
            CurrentFrequencyTextMesh.SetText($"{(CurrentState / 10d):0.0}");
        }

        public override void Update()
        {
            base.Update();
            if (!IsRadioOn)
            {
                CurrentFrequencyTextMesh.SetText("");
                BackGroundAudioSource.Stop();
                ChannelAudioSource.Stop();
            }
            else
            {
                int roundedFreq = RoundedFrequenceyIfNeeded();
                ShowLerpedFrequencyText();
                if (!BackGroundAudioSource.isPlaying)
                    BackGroundAudioSource.Play();
            }

            UpdateNoiseAndMessageVolumes();
        }

        private void UpdateNoiseAndMessageVolumes()
        {
            int roundedState = RoundedFrequenceyIfNeeded();
            for (var i = -30; i <= 30; ++i)
            {
                if (Frequencies.ContainsKey(roundedState + i))
                {
                    ChannelAudioSource.volume = (1.0f - Mathf.Abs((float)i) / 30.0f) * .3f;
                    BackGroundAudioSource.volume = .2f * Mathf.Abs((float) i) / 30.0f;
                    if (!_channelIsPlaying)
                    {
                        ChannelAudioSource.clip = Frequencies[roundedState + i];
                        ChannelAudioSource.Play();
                        _channelIsPlaying = true;
                    }

                    _channelIsPlaying = ChannelAudioSource.isPlaying;
                    return;
                }
            }
            ChannelAudioSource.Stop();
        }

        private int RoundedFrequenceyIfNeeded()
        {
            for(int i = 0; i < RoundToNumbers.Length; ++i)
            {
                if (Math.Abs(CurrentState - RoundToNumbers[i]) <= 4)
                {
                    return RoundToNumbers[i];
                }
            }
            return CurrentState;
        }

        private void ShowLerpedFrequencyText()
        {
            if (currentlyVisibleFrequency != CurrentState)
            {
                currentlyVisibleFrequency += currentlyVisibleFrequency > CurrentState ? -1 : 1;
                CurrentFrequencyTextMesh.SetText($"{(currentlyVisibleFrequency / 10d):0.0}");
            }
        }
    }
}