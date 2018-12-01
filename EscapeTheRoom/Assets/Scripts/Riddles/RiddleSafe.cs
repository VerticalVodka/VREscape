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
        }

        public void StartRiddle()
        {
            Debug.Log("RiddleSafe started");
            _combinationProgress = 0;
            StartCoroutine(SafeRoutine());
        }

        private void Update()
        {
        }

        private IEnumerator SafeRoutine()
        {
            Debug.Log($"SafeRotaryValue :{SafeRotary.CurrentState}");
            while (_combinationProgress < Combination.Count)
            {

                if (_combinationProgress > 0 && ((Combination[_combinationProgress - 1] < 0 && SafeRotary.CurrentState < Combination[_combinationProgress - 1])
                    || (Combination[_combinationProgress - 1] > 0 && SafeRotary.CurrentState > Combination[_combinationProgress - 1])))
                { // Mistakes were made
                    Debug.Log("Mistake made");
                    _combinationProgress = 0;
                    yield return new WaitForSecondsRealtime(0);
                }
                else if (Combination[_combinationProgress] != SafeRotary.CurrentState)
                {
                    yield return new WaitForSecondsRealtime(0);
                }
                else
                {
                    Debug.Log("Combination " + _combinationProgress + " found");
                    if (_combinationProgress < Combination.Count - 1)
                    {
                        Debug.Log($"Next Target: {Combination[_combinationProgress + 1]}");
                    }
                    _combinationProgress++;
                    yield return new WaitForSecondsRealtime(0);
                }
            }
            Debug.Log("RiddleSafe solved");
            OnRiddleDone?.Invoke(true);
        }
    }
}
