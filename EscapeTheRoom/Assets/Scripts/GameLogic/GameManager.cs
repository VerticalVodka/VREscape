using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VREscape
{
    public class GameManager : MonoBehaviour
    {
        private IList<IRiddle> riddles;
        private IEnumerator<IRiddle> currentRiddle;

        private void NextRiddle()
        {
            if (currentRiddle.MoveNext())
            {
                currentRiddle.Current.OnRiddleDone += OnRiddleDoneListener;
            }
        }

        void OnRiddleDoneListener(bool value)
        {
            currentRiddle.Current.OnRiddleDone -= OnRiddleDoneListener;
            NextRiddle();
        }

        // Use this for initialization
        void Start()
        {
            riddles = new List<IRiddle>() { new RiddleRadio(), new RiddleRadio(), new RiddleRadio() };
            currentRiddle = riddles.GetEnumerator();
            NextRiddle();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}