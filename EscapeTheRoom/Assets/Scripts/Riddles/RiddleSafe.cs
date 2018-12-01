using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VREscape
{
    public class RiddleSafe : MonoBehaviour, IRiddle
    {
        public enum Directions { ClockWise, CounterClockWise };
        public event Action<bool> OnRiddleDone;
        public SafeRotary SafeRotary;
        public AudioClip CombinationDigitMatchesSound;
        public List<Tuple<int, Directions>> Combination = new List<Tuple<int, Directions>>();
        public int CombinationLength;

        private AudioSource _audioSource;
        private int _combinationProgress;

        private void Start()
        {
            Debug.Log("RiddleSafe started");
            _combinationProgress = 0;
            _audioSource = GetComponent<AudioSource>();
            StartRiddle();
        }

        public void StartRiddle()
        {
            System.Random rand = new System.Random();
            for (int i = 0; i < CombinationLength; ++i)
            {
                Combination.Add(new Tuple<int, Directions>(rand.Next(SafeRotary.MinValue, SafeRotary.MaxValue), rand.Next(2) == 0 ? Directions.ClockWise : Directions.CounterClockWise));
            }
            StartCoroutine(SafeRoutine());
        }

        private void Update()
        {
        }

        private Directions GetDirectionOfTurn(SafeRotary safeRotary)
        {
            if (safeRotary.LastState < safeRotary.CurrentState) return Directions.ClockWise;
            else return Directions.CounterClockWise;
        }

        private IEnumerator SafeRoutine()
        {
            Debug.Log($"SafeRotaryValue :{SafeRotary.CurrentState}");
            while (_combinationProgress < Combination.Count)
            {
                if(!SafeRotary.HasTurned())
                {
                    yield return new WaitForSecondsRealtime(0);
                }

                if (GetDirectionOfTurn(SafeRotary) != Combination[_combinationProgress].Item2)
                { // Mistakes were made
                    Debug.Log("Mistake made");
                    _combinationProgress = 0;
                    yield return new WaitForSecondsRealtime(0);
                }
                else if (Combination[_combinationProgress].Item1 != SafeRotary.CurrentState)
                {
                    yield return new WaitForSecondsRealtime(0);
                }
                else
                {
                    Debug.Log("Combination " + _combinationProgress + " found");
                    _audioSource.PlayOneShot(CombinationDigitMatchesSound);
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
