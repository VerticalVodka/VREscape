﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace VREscape
{
    public class RiddleSafe : MonoBehaviour, IRiddle
    {
        public enum Directions { ClockWise, CounterClockWise };
        public event Action<bool> OnRiddleDone;
        public GameObject InitiallyHiddenStuff;
        public SafeRotary SafeRotary;
        public AudioClip CombinationDigitMatchesSound;
        public AudioClip FailSound;
        public List<Tuple<int, Directions>> Combination = new List<Tuple<int, Directions>>();
        private int _combinationLength;
        public GameObject TurnClockWisePlane;
        public GameObject TurnCounterClockWisePlane;
        public Boolean AutoStartRiddle = false;
        public Boolean isDebug = false;
        public List<GameObject> Planes;


        private AudioSource _audioSource;
        private int _combinationProgress;

        private void Start()
        {
            Debug.Log("RiddleSafe started");
            _combinationProgress = 0;
            _combinationLength = Planes.Count;
            _audioSource = GetComponent<AudioSource>();
            //InitiallyHiddenStuff.GetComponentsInChildren<MeshRenderer>().ToList().ForEach(s => s.GetComponent<MeshRenderer>().enabled = false);
            InitiallyHiddenStuff.SetActive(false);
            if (AutoStartRiddle) StartRiddle();
        }

        public void StartRiddle()
        {
            //InitiallyHiddenStuff.GetComponentsInChildren<MeshRenderer>().ToList().ForEach(s => s.GetComponent<MeshRenderer>().enabled = true);
            InitiallyHiddenStuff.SetActive(true);
            System.Random rand = new System.Random();

            for (int i = 0; i < _combinationLength; ++i)
            {
                Directions nextDirection = rand.Next(2) == 0 ? Directions.ClockWise : Directions.CounterClockWise;
                int nextVal = nextDirection == Directions.ClockWise ? rand.Next(SafeRotary.MaxValue - 1) + 1 : (rand.Next(Math.Abs(SafeRotary.MaxValue) - 1) + 1) * -1;
                Combination.Add(new Tuple<int, Directions>(nextVal, nextDirection));
                Material material;
                if (nextDirection == Directions.CounterClockWise)
                    material = TurnClockWisePlane.GetComponent<MeshRenderer>().sharedMaterial;
                else
                    material = TurnCounterClockWisePlane.GetComponent<MeshRenderer>().sharedMaterial;
                Planes[i].GetComponent<MeshRenderer>().material = material;
            }
            if (isDebug)
            {
                String comb = "Combination\n";
                foreach (var t in Combination)
                {
                    comb += $"{t.Item2} {t.Item1} \n";
                }
                Debug.Log(comb);
            }
            UpdateDirectionPlanes();
            StartCoroutine(SafeRoutine());
        }

        private Directions GetDirectionToTurn()
        {
            if (_combinationProgress < Combination.Count)
            {
                return Combination[_combinationProgress].Item2;
            }
            return Directions.ClockWise; // Just for Safety
        }

        private void UpdateDirectionPlanes()
        {
            if (GetDirectionToTurn() == Directions.ClockWise)
            {
                TurnClockWisePlane.GetComponent<MeshRenderer>().enabled = true;
                TurnCounterClockWisePlane.GetComponent<MeshRenderer>().enabled = false;
                if (isDebug) Debug.Log("Enabled Plane Clockwise");
            }
            else
            {
                TurnClockWisePlane.GetComponent<MeshRenderer>().enabled = false;
                TurnCounterClockWisePlane.GetComponent<MeshRenderer>().enabled = true;
                if (isDebug) Debug.Log("Enabled Plane Counterclockwise");
            }
        }

        private Directions GetDirectionOfTurn(SafeRotary safeRotary)
        {
            if (safeRotary.LastState == safeRotary.MinValue && safeRotary.CurrentState == safeRotary.MaxValue)
            {
                if (isDebug) Debug.Log("CCW-flip");
                return Directions.CounterClockWise;
            }
            else if (safeRotary.LastState == safeRotary.MaxValue && safeRotary.CurrentState == safeRotary.MinValue)
            {
                if (isDebug) Debug.Log("CW-flip");
                return Directions.ClockWise;
            }
            else if (safeRotary.LastState < safeRotary.CurrentState)
            {
                if (isDebug) Debug.Log("CW");
                return Directions.ClockWise;
            }
            else
            {
                if (isDebug) Debug.Log("CCW");
                return Directions.CounterClockWise;
            }
        }

        private IEnumerator SafeRoutine()
        {
            while (_combinationProgress < Combination.Count)
            {
                if (!SafeRotary.HasTurned())
                {
                    //if (isDebug) Debug.Log("NoTurn");
                    yield return new WaitForSecondsRealtime(0);

                }
                else if (GetDirectionOfTurn(SafeRotary) != Combination[_combinationProgress].Item2)
                { // Mistakes were made
                    if (isDebug) Debug.Log("Mistake made");
                    _combinationProgress = 0;
                    UpdateDirectionPlanes();
                    _audioSource.PlayOneShot(FailSound);
                    yield return new WaitForSecondsRealtime(0);
                }
                else if (Combination[_combinationProgress].Item1 != SafeRotary.CurrentState)
                {
                    yield return new WaitForSecondsRealtime(0);
                }
                else
                {
                    if (isDebug) Debug.Log("Combination found");
                    _audioSource.PlayOneShot(CombinationDigitMatchesSound);
                    if (_combinationProgress < Combination.Count - 1)
                    {
                        if (isDebug) Debug.Log($"Next Target: {Combination[_combinationProgress + 1]}");
                    }
                    _combinationProgress++;
                    UpdateDirectionPlanes();
                    yield return new WaitForSecondsRealtime(0);
                }
            }
            if (isDebug) Debug.Log("RiddleSafe solved");
            OnRiddleDone?.Invoke(true);
        }
    }
}
