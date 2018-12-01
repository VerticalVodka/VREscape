using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VREscape
{
    public class RiddleSafe : MonoBehaviour, IRiddle
    {
        public event Action<bool> OnRiddleDone;
        public SafeRotary SafeRotary;
        public AudioClip CombinationDigitMatchesSound;
        public List<int> Combination = new List<int>();

        private int _combinationProgress;

        private void Start()
        {
            Debug.Log("RiddleSafe started");
            _combinationProgress = 0;
            StartRiddle();
        }

        public void StartRiddle()
        {
            StartCoroutine(SafeRoutine());
            Debug.Log("RiddleSafe solved");
            OnRiddleDone?.Invoke(true);
        }

        private void Update()
        {
        }

        private IEnumerator SafeRoutine()
        {
            while (_combinationProgress < Combination.Count)
            {

                Debug.Log($"SafeRotaryValue :{SafeRotary.CurrentState}");
                Debug.Log($"Next Target: {Combination[_combinationProgress]}");
                if (_combinationProgress > 0 && ((Combination[_combinationProgress - 1] < 0 && SafeRotary.CurrentState < Combination[_combinationProgress - 1])
                    || (Combination[_combinationProgress - 1] > 0 && SafeRotary.CurrentState > Combination[_combinationProgress - 1])))
                { // Mistakes were made
                    Debug.Log("Mistake made");
                    _combinationProgress = 0;
                    yield return new WaitForSecondsRealtime(0);
                }
                else if (Combination[_combinationProgress] != SafeRotary.CurrentState)
                {
                    Debug.Log("Not there yet");
                    yield return new WaitForSecondsRealtime(0);
                }
                else
                {
                    Debug.Log("Combination " + _combinationProgress + " found");
                    _combinationProgress++;
                    yield return new WaitForSecondsRealtime(0);
                }
            }
        }
    }
}
