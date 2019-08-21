using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VREscape
{
    public class RiddleRadio : MonoBehaviour, IRiddle
    {
        private RadioRotary radioRotary;
        private HWManager hwManager;
        private AudioSource buttonAudioSource;
        private bool isActive = false;
        private readonly bool[] isPressed = new bool[4];

		public RadioRandomBackgroundNoise RandomBackgroundNoise;
		
		public AudioSource ElevatorMusicSource;
        public AudioClip RiddleInstruction;
        public AudioClip Instruction927Start;
        public AudioClip Instruction1104Clue1;
        public AudioClip Instruction0800Clue2;
        public AudioClip CorrectButton;
        public AudioClip WrongButton;

        private AudioSource instructionSource;

        public Enums.ButtonEnum[] FirstCorrectButtonOrder = new[]
        {
            Enums.ButtonEnum.Button1,
            Enums.ButtonEnum.Button1,
            Enums.ButtonEnum.Button2
        };

        public Enums.ButtonEnum[] SecondCorrectButtonOrder = new[]
        {
            Enums.ButtonEnum.Button3,
            Enums.ButtonEnum.Button2,
            Enums.ButtonEnum.Button4
        };

        private int currentButtonIdx;
        private bool solvedSequence1 = false;
        private bool solvedSequence2 = false;
        private readonly int maxDifferenceToBeNearFrequency = 25;
        Enums.ButtonEnum[] wantedSequence = null;

        public event Action<bool> OnRiddleDone;

        public void StartRiddle()
        {
            Debug.Log("RiddleRadio started");
			RandomBackgroundNoise.StartNoise();
            hwManager = FindObjectOfType<HWManager>();
            isActive = true;
            radioRotary = FindObjectOfType<RadioRotary>();
            buttonAudioSource = GetComponent<AudioSource>();
            instructionSource = GetComponent<AudioSource>();
            ResetFrequency();
            radioRotary.IsRadioOn = true;
            radioRotary.Frequencies.Add(927, Instruction927Start);
			radioRotary.Frequencies.Add(1000,RiddleInstruction);
            instructionSource.Play();
			ElevatorMusicSource.Stop();
            StartCoroutine(Do());
        }

        private void ResetFrequency()
        {
            radioRotary.Reset();
        }

        public void SkipRiddle()
        {
            Debug.Log("Skipped Radio Riddle");
            FinishLevel();
        }

        public void Update()
        {
            if (!isActive || radioRotary.Frequencies.Count <= 1 || wantedSequence == null || currentButtonIdx >= wantedSequence.Length)
                return;

            var pressedButton = GetPressedButton();
            if (pressedButton == null) return;

            if (pressedButton == wantedSequence[currentButtonIdx])
            {
                ++currentButtonIdx;
                buttonAudioSource.PlayOneShot(CorrectButton);
                CheckForRiddleSolved();
            } else
            {
                currentButtonIdx = 0;
                buttonAudioSource.PlayOneShot(WrongButton);
            }
        }

        private Enums.ButtonEnum? GetPressedButton()
        {
            var i = 0;
            foreach (var currentButton in new [] {Enums.ButtonEnum.Button1, Enums.ButtonEnum.Button2, Enums.ButtonEnum.Button3, Enums.ButtonEnum.Button4 })
            {
                
                if (hwManager.GetButtonState(currentButton))
                {
                    if (!isPressed[i])
                    {
                        isPressed[i] = true;
                        return currentButton;
                    }
                }
                else
                {
                    isPressed[i] = false;
                }
                i++;
            }
            return null;
        }

        private IEnumerator Do()
        {
            while (!NearFreq(927, 10))
                yield return new WaitForSecondsRealtime(0);
            Debug.Log("Right Frequence");
            radioRotary.Frequencies.Add(1104, Instruction1104Clue1);
            radioRotary.Frequencies.Add(800, Instruction0800Clue2);

            while (!RiddleIsSolved())
            {
                if (NearFreq(1104))
                {
                    SetWantedButtonSequenceTo(FirstCorrectButtonOrder);
                } else if (NearFreq(800))
                {
                    SetWantedButtonSequenceTo(SecondCorrectButtonOrder);
                } else
                {
                    SetToNoWantedButtonSequence();
                }

                yield return new WaitForSecondsRealtime(0);
            }

            Debug.Log("Solved Riddle");
            FinishLevel();
        }

        private bool RiddleIsSolved()
        {
            return solvedSequence1 && solvedSequence2;
        }

        private bool NearFreq(int frequency, int? overrideMaxDiff = null)
        {
            return Math.Abs(radioRotary.CurrentState - frequency) < (overrideMaxDiff ?? maxDifferenceToBeNearFrequency);
        }

        private void CheckForRiddleSolved()
        {
            if (currentButtonIdx > wantedSequence.Length)
            {
                if (wantedSequence == FirstCorrectButtonOrder)
                {
                    Debug.Log("Solved Sequence 1");
                    solvedSequence1 = true;
                }

                if (wantedSequence == SecondCorrectButtonOrder)
                {
                    Debug.Log("Solved Sequence 2");
                    solvedSequence2 = true;
                }
            }
        }

        private void SetWantedButtonSequenceTo(Enums.ButtonEnum[] sequence)
        {
            if (wantedSequence != sequence)
            {
                currentButtonIdx = 0;
                wantedSequence = sequence;
            }
        }

        private void SetToNoWantedButtonSequence()
        {
            currentButtonIdx = 0;
            wantedSequence = null;
        }

        private void FinishLevel()
        {
			RandomBackgroundNoise.StopNoise();
            radioRotary.Frequencies.Clear();
            radioRotary.IsRadioOn = false;
            OnRiddleDone?.Invoke(true);
        }

        private bool RiddleSolved(int correctButtonLength)
        {
            return currentButtonIdx >= correctButtonLength;
        }
    }
}