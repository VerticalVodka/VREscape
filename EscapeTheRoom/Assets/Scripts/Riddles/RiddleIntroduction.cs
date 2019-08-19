using System;
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
			Debug.Log("1");
            StartCoroutine(LinearVolumeUpOfSource(AmbienceSource, TimeToVolumeUpOfAmbience));

            yield return new WaitForSecondsRealtime(DelayBetweenAmbienceToElevatorMusic);
			Debug.Log("2");
            StartCoroutine(LinearVolumeUpOfSource(ElevatorSource, TimeToVolumeUpOfElevator));

            yield return new WaitForSecondsRealtime(DelayBetweenElevatorMusicAndIntroduction);
			Debug.Log("3");
            IntroductionSource.PlayOneShot(introductionClip);

            yield return new WaitForSecondsRealtime((int)(introductionClip.length * 1000) + 1);
            FinishRiddle();
        }

        private IEnumerator LinearVolumeUpOfSource(AudioSource source, float timeToVolumeUp)
        {
			for (int i = 1; i <= 100; ++i) {
				source.volume = ((float)i) / 100;
				Debug.Log("Volume set to " + (((float)i) / 100));
				yield return new WaitForSecondsRealtime(timeToVolumeUp / 100);
			}
        }

        public void Start()
        {
            hwManager = FindObjectOfType<HWManager>();
        }
    }
}
