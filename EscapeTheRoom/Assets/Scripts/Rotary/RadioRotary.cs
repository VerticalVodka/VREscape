using System.Collections.Generic;
using UnityEngine;

namespace VREscape
{
    public class RadioRotary : Rotary
    {
        public bool IsRadioOn = false;
        public IDictionary<int, AudioClip> Frequencies = new Dictionary<int, AudioClip>();
        public AudioSource ChannelAudioSource;
        private bool _channelIsPlaying = false;
        public AudioSource BackGroundAudioSource;

        public void Start()
        {
            base.Start();
        }

        public void Update()
        {
            base.Update();
            if (!IsRadioOn)
            {
                BackGroundAudioSource.Stop();
                ChannelAudioSource.Stop();
            }
            else
            {
                if (!BackGroundAudioSource.isPlaying)
                    BackGroundAudioSource.Play();
            }

            for (var i = -5; i <= 5; ++i)
            {
                if (Frequencies.ContainsKey(CurrentState + i))
                {
                    ChannelAudioSource.volume = 1.0f - Mathf.Abs((float)i) / 5.0f;
                    BackGroundAudioSource.volume = Mathf.Abs((float) i) / 5.0f;
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