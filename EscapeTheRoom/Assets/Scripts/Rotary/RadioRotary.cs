using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace VREscape
{
    public class RadioRotary : Rotary
    {
        public bool IsRadioOn = false;
        public IDictionary<int, AudioClip> Frequencies = new Dictionary<int, AudioClip>();
        public AudioSource ChannelAudioSource;
        private bool _channelIsPlaying = false;
        public AudioSource BackGroundAudioSource;

        public TextMeshPro CurrentFrequency;

        new protected readonly int changeMultiplier = -1;

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
                CurrentFrequency.SetText($"{(CurrentState / 10d):0.0}");
                if (!BackGroundAudioSource.isPlaying)
                    BackGroundAudioSource.Play();
            }

            for (var i = -5; i <= 5; ++i)
            {
                if (Frequencies.ContainsKey(CurrentState + i))
                {
                    ChannelAudioSource.volume = (1.0f - Mathf.Abs((float)i) / 5.0f) * .3f;
                    BackGroundAudioSource.volume = .2f * Mathf.Abs((float) i) / 5.0f;
                    if (!_channelIsPlaying)
                    {
                        ChannelAudioSource.clip = Frequencies[CurrentState + i];
                        ChannelAudioSource.Play();
                        _channelIsPlaying = true;
                    }

                    _channelIsPlaying = ChannelAudioSource.isPlaying;
                }
            }
        }
    }
}