using System;
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

        public AudioSource audioSource;
        public AudioClip wakeUpClip;
        public AudioClip introductionClip;
        public List<Enums.ButtonEnum> buttonsThatStart;
        public int wakeUpDelay = 1500;

        private HWManager hwManager;
        private bool done = false;
        private bool wakeUpFinished = false;
        private bool shouldPlayWakeupClip = false;

        public void StartRiddle()
        {
            PlayWakeUpClip();
        }

        private void PlayWakeUpClip()
        {
            wakeUpFinished = false;
            shouldPlayWakeupClip = true;
        }

        private void StopWakeUpClip()
        {
            shouldPlayWakeupClip = false;
        }

        private void PlayIntroductionAndThenFinish()
        {
            audioSource.PlayOneShot(introductionClip);

            int waitLength = (int)(introductionClip.length * 1000) + 1;
            Task waitForClipFinished = new Task(async () =>
            {
                await Task.Delay(waitLength);
                done = true;
            });
            waitForClipFinished.Start();
        }

        public void Start()
        {
            hwManager = FindObjectOfType<HWManager>();
        }

        public void Update()
        {
            if (!done)
            {
                if (!wakeUpFinished && shouldPlayWakeupClip)
                {
                    shouldPlayWakeupClip = false;
                    audioSource.PlayOneShot(wakeUpClip);

                    int waitLength = (int)(wakeUpClip.length * 1000) + wakeUpDelay;
                    Task waitForClipFinished = new Task(async () =>
                    {
                        await Task.Delay(waitLength);
                        shouldPlayWakeupClip = true;
                    });
                    waitForClipFinished.Start();
                }

                if (buttonsThatStart.Any(btn => hwManager.GetButtonState(btn)))
                {
                    wakeUpFinished = true;
                    PlayIntroductionAndThenFinish();
                }
            }
            else if (OnRiddleDone != null)
                OnRiddleDone.Invoke(true);
        }
    }
}
