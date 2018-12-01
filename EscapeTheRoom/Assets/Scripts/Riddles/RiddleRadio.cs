using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VREscape
{
    public class RiddleRadio : MonoBehaviour, IRiddle
    {
        private RadioRotary _radioRotary;
        private HWManager _hwManager;
        private bool _isActive = false;

        public AudioClip RiddleInstruction;
        public AudioClip Instruction1441Start;
        public AudioClip Instruction1042Clue1;
        public AudioClip Instruction0815Clue2;
        public AudioClip CorrectButton;
        public AudioClip WrongButton;
        public AudioSource ButtonAudioSource;

        private AudioSource _instructionSource;

        public Enums.ButtonEnum[] CorrectButtonOrder = new[]
        {
            Enums.ButtonEnum.Button3,
            Enums.ButtonEnum.Button2,
            Enums.ButtonEnum.Button4
        };

        private int _currentButton = 0;

        public event Action<bool> OnRiddleDone;

        public void StartRiddle()
        {
            Debug.Log("RiddleRadio started");
            _hwManager = FindObjectOfType<HWManager>();
            _isActive = true;
            _radioRotary = GetComponentInChildren<RadioRotary>();
            _instructionSource = GetComponent<AudioSource>();
            _radioRotary.Frequencies.Add(1441, Instruction1441Start);
            _instructionSource.clip = RiddleInstruction;
            _instructionSource.Play();
            StartCoroutine(Do());
            OnRiddleDone?.Invoke(true);
        }

        public void Update()
        {
            if (!_isActive) return;
            var pressedButton = GetPressedButton();
            if (pressedButton == null) return;
            if (pressedButton == CorrectButtonOrder[_currentButton])
            {
                _currentButton++;
                ButtonAudioSource.clip = CorrectButton;
                ButtonAudioSource.Play();
            }
            else
            {
                _currentButton = 0;
                ButtonAudioSource.clip = WrongButton;
                ButtonAudioSource.Play();
            }
        }

        private Enums.ButtonEnum? GetPressedButton()
        {
            foreach (var currentButton in (Enums.ButtonEnum[]) Enum.GetValues(typeof(Enums.ButtonEnum)))
            {
                if (_hwManager.GetButtonState(currentButton))
                {
                    return currentButton;
                }
            }

            return null;
        }

        private IEnumerator Do()
        {
            while (_radioRotary.CurrentState != 1441)
                yield return new WaitForSecondsRealtime(0);
            _radioRotary.Frequencies.Add(1042, Instruction1042Clue1);
            _radioRotary.Frequencies.Add(0815, Instruction0815Clue2);
            while (!RiddleSolved())
                yield return new WaitForSecondsRealtime(0);
            _radioRotary.Frequencies.Clear();
        }

        private bool RiddleSolved()
        {
            return _currentButton >= CorrectButtonOrder.Length;
        }
    }
}