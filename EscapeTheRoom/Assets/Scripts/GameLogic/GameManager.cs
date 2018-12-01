using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VREscape
{
    public class GameManager : MonoBehaviour
    {
        public List<IRiddle> Riddles = new List<IRiddle>();
        private IEnumerator<IRiddle> riddleEnumerator;

        private void NextRiddle()
        {
            if (riddleEnumerator.MoveNext())
            {
                riddleEnumerator.Current.OnRiddleDone += OnRiddleDoneListener;
                riddleEnumerator.Current.StartRiddle();
            }
        }

        void OnRiddleDoneListener(bool value)
        {
            riddleEnumerator.Current.OnRiddleDone -= OnRiddleDoneListener;

            NextRiddle();
        }

        // Use this for initialization
        void Start()
        {
            try
            {
                Riddles.Add(FindObjectOfType<RiddleSafe>());
                Riddles.Add(FindObjectOfType<RiddleRadio>());
                riddleEnumerator = Riddles.GetEnumerator();
                NextRiddle();
            }
            catch (Exception e)
            {
                Debug.Log("GameManager Failed");
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}