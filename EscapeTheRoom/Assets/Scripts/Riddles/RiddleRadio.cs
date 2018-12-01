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

        public event Action<bool> OnRiddleDone;

        public void StartRiddle()
        {
            Debug.Log("RiddleRadio started");
            _hwManager = FindObjectOfType<HWManager>();
            _isActive = true;
            _radioRotary = GetComponentInChildren<RadioRotary>();
            _radioRotary.Frequencies.Add(1441, RiddleInstruction);
            StartCoroutine(Do());
            OnRiddleDone?.Invoke(true);
        }

        public void Update()
        {
            if (!_isActive) return;
        }

        private IEnumerator Do()
        {
            while (_radioRotary.CurrentValue != 1441)
                yield return new WaitForSecondsRealtime(0);
            _radioRotary.Frequencies.Add(1042, Instruction1042Clue1);
            _radioRotary.Frequencies.Add(0815, Instruction0815Clue2);
            while (RiddleSolved())
                yield return new WaitForSecondsRealtime(0);
        }

        private bool RiddleSolved()
        {
            return true;
        }
    }
}