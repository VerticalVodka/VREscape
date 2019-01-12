using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace VREscape
{
    public class GameManager : MonoBehaviour
    {
        public List<IRiddle> Riddles = new List<IRiddle>();
        private IEnumerator<IRiddle> riddleEnumerator;

        public int PauseBetweenRiddlesInMs = 1000;

        private bool goToNextRiddle = false;

        private void NextRiddle()
        {
            if (riddleEnumerator.MoveNext())
            {
                Task waitBeforeNextRiddle = new Task(async () =>
                {
                    if (PauseBetweenRiddlesInMs > 0)
                        await Task.Delay(PauseBetweenRiddlesInMs);
                    goToNextRiddle = true;
                });
                waitBeforeNextRiddle.Start();
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
                Riddles.Add(FindObjectOfType<SimonSaysRiddle>());
                Riddles.Add(FindObjectOfType<RiddleRadio>());
                Riddles.Add(FindObjectOfType<RiddleFlashlight>());
                Riddles.Add(FindObjectOfType<RiddleSafe>());
                riddleEnumerator = Riddles.GetEnumerator();
                NextRiddle();
            }
            catch (Exception)
            {
                Debug.Log("GameManager Failed");
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (goToNextRiddle)
            {
                Debug.Log("Starting the next riddle of type " + riddleEnumerator.Current.GetType().ToString());
                goToNextRiddle = false;
                riddleEnumerator.Current.OnRiddleDone += OnRiddleDoneListener;
                riddleEnumerator.Current.StartRiddle();
            }
        }
    }

}