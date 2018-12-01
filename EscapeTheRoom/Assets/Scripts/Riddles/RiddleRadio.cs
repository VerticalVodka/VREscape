﻿using System;
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

        public AudioClip RiddleInstruction;
        public AudioClip Instruction927Start;
        public AudioClip Instruction1104Clue1;
        public AudioClip Instruction0800Clue2;
        public AudioClip CorrectButton;
        public AudioClip WrongButton;

        private AudioSource _instructionSource;

        public Enums.ButtonEnum[] CorrectButtonOrder = new[]
        {
            Enums.ButtonEnum.Button3,
            Enums.ButtonEnum.Button2,
            Enums.ButtonEnum.Button4
        };

        private int _currentButton;

        public event Action<bool> OnRiddleDone;

        public void Start()
        {
        }

        public void StartRiddle()
        {
            Debug.Log("RiddleRadio started");
            _hwManager = FindObjectOfType<HWManager>();
            _isActive = true;
            _radioRotary = FindObjectOfType<RadioRotary>();
            _buttonAudioSource = GetComponent<AudioSource>();
            _instructionSource = GetComponent<AudioSource>();
            _radioRotary.Frequencies.Add(927, Instruction927Start);
            _instructionSource.clip = RiddleInstruction;
            _instructionSource.Play();
            StartCoroutine(Do());
            OnRiddleDone?.Invoke(true);
        }

        public void Update()
        {
            //Debug.Log(_radioRotary.CurrentState);
            if (!_isActive) return;
            var pressedButton = GetPressedButton();
            if (pressedButton == null) return;
            //Debug.Log(pressedButton);
            if (pressedButton == CorrectButtonOrder[_currentButton])
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
            _radioRotary.Frequencies.Add(0800, Instruction0800Clue2);
            while (!RiddleSolved())
                yield return new WaitForSecondsRealtime(0);
            Debug.Log("Solved Riddle");
            _radioRotary.Frequencies.Clear();
        }

        private bool RiddleSolved()
        {
            return _currentButton >= CorrectButtonOrder.Length;
        }
    }
}