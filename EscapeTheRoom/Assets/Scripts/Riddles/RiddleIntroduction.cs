﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VREscape;

namespace Assets.Scripts.Riddles
{
    public class RiddleIntroduction : MonoBehaviour, IRiddle
    {
        public event Action<bool> OnRiddleDone;

        public AudioSource IntroductionSource;
        public AudioSource AmbienceSource;
        public AudioSource ElevatorSource;
        public AudioClip introductionClip;
        public List<Enums.ButtonEnum> buttonsThatStart;

        private HWManager hwManager;

        public void StartRiddle()
        {
            StartCoroutine(PlaySoundsCoroutine());
        }

        private void FinishRiddle()
        {
            OnRiddleDone?.Invoke(true);
        }

        public void SkipRiddle()
        {
            StopAllCoroutines();
            FinishRiddle();
        }

        public float DelayBeforeStart = 1.5f;
        public float TimeToVolumeUpOfAmbience = 0.5f;
        public float DelayBetweenAmbienceToElevatorMusic = 1.5f;
        public float TimeToVolumeUpOfElevator = 0.5f;
        public float DelayBetweenElevatorMusicAndIntroduction = 1.5f;

        private IEnumerator PlaySoundsCoroutine()
        {
            yield return new WaitForSecondsRealtime(DelayBeforeStart);
            LinearVolumeUpOfSource(AmbienceSource, TimeToVolumeUpOfAmbience);

            yield return new WaitForSecondsRealtime(DelayBetweenAmbienceToElevatorMusic);
            LinearVolumeUpOfSource(ElevatorSource, TimeToVolumeUpOfElevator);

            yield return new WaitForSecondsRealtime(DelayBetweenElevatorMusicAndIntroduction);
            IntroductionSource.PlayOneShot(introductionClip);

            yield return new WaitForSecondsRealtime((int)(introductionClip.length * 1000) + 1);
            FinishRiddle();
        }

        private void LinearVolumeUpOfSource(AudioSource source, float timeToVolumeUp)
        {
            var t = new Task(() =>
            {
                for (int i = 0; i <= 50; ++i)
                    source.volume = i * (50 / timeToVolumeUp);
            });
            t.Start();
        }

        public void Start()
        {
            hwManager = FindObjectOfType<HWManager>();
        }
    }
}
