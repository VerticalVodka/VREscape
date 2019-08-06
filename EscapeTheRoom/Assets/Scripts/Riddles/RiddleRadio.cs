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
        private AudioSource _buttonAudioSource;
        private bool _isActive = false;
        private readonly bool[] _isPressed = new bool[4];

		public RadioRandomBackgroundNoise RandomBackgroundNoise;
		
		public AudioSource ElevatorMusicSource;
        public AudioClip RiddleInstruction;
        public AudioClip Instruction927Start;
        public AudioClip Instruction1104Clue1;
        public AudioClip Instruction0800Clue2;
        public AudioClip CorrectButton;
        public AudioClip WrongButton;

        private AudioSource _instructionSource;

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

        private int _currentButton;
        private bool _solvedFirstButtonOrder = false;
        private bool _allowButtons = false;

        public event Action<bool> OnRiddleDone;

        public void Start()
        {
        }

        public void StartRiddle()
        {
            Debug.Log("RiddleRadio started");
			RandomBackgroundNoise.StartNoise();
            _hwManager = FindObjectOfType<HWManager>();
            _isActive = true;
            _radioRotary = FindObjectOfType<RadioRotary>();
            _buttonAudioSource = GetComponent<AudioSource>();
            _instructionSource = GetComponent<AudioSource>();
            _radioRotary.IsRadioOn = true;
            _radioRotary.Frequencies.Add(927, Instruction927Start);
			_radioRotary.Frequencies.Add(1000,RiddleInstruction);
            _instructionSource.Play();
			ElevatorMusicSource.Stop();
            StartCoroutine(Do());
        }

        public void SkipRiddle()
        {
            Debug.Log("Skipped Radio Riddle");
            FinishLevel();
        }

        public void Update()
        {
            //Debug.Log(_radioRotary.CurrentState);
            if (!_isActive || _radioRotary.Frequencies.Count <= 1 || !_allowButtons) return;

            var pressedButton = GetPressedButton();
            if (pressedButton == null) return;
            //Debug.Log(pressedButton);
            Enums.ButtonEnum[] correctButtonOrder;
            if (!_solvedFirstButtonOrder)
            {
                correctButtonOrder = FirstCorrectButtonOrder;
            } else
            {
                correctButtonOrder = SecondCorrectButtonOrder;
            }

            if (pressedButton == correctButtonOrder[_currentButton])
            {
                _currentButton++;
                _buttonAudioSource.clip = CorrectButton;
                _buttonAudioSource.Play();
            }
            else
            {
                _currentButton = 0;
                _buttonAudioSource.clip = WrongButton;
                _buttonAudioSource.Play();
            }
        }

        private Enums.ButtonEnum? GetPressedButton()
        {
            var i = 0;
            foreach (var currentButton in new [] {Enums.ButtonEnum.Button1, Enums.ButtonEnum.Button2, Enums.ButtonEnum.Button3, Enums.ButtonEnum.Button4 })
            {
                
                if (_hwManager.GetButtonState(currentButton))
                {
                    if (!_isPressed[i])
                    {
                        _isPressed[i] = true;
                        return currentButton;
                    }
                }
                else
                {
                    _isPressed[i] = false;
                }
                i++;
            }
            return null;
        }

        private IEnumerator Do()
        {
            while (_radioRotary.CurrentState != 927)
                yield return new WaitForSecondsRealtime(0);
            Debug.Log("Right Frequence");
            _radioRotary.Frequencies.Add(1104, Instruction1104Clue1);

            while (_radioRotary.CurrentState != 1104)
                yield return new WaitForSecondsRealtime(0);
            _allowButtons = true;

            while (!RiddleSolved(FirstCorrectButtonOrder.Length))
                yield return new WaitForSecondsRealtime(0);
            _solvedFirstButtonOrder = true;
            _currentButton = 0;
            _radioRotary.Frequencies.Add(800, Instruction0800Clue2);

            while (!RiddleSolved(SecondCorrectButtonOrder.Length))
                yield return new WaitForSecondsRealtime(0);
            Debug.Log("Solved Riddle");
            FinishLevel();
        }

        private void FinishLevel()
        {
			RandomBackgroundNoise.StopNoise();
            _radioRotary.Frequencies.Clear();
            _radioRotary.IsRadioOn = false;
            OnRiddleDone?.Invoke(true);
        }

        private bool RiddleSolved(int correctButtonLength)
        {
            return _currentButton >= correctButtonLength;
        }
    }
}