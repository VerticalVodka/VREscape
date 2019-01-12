using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace VREscape
{
    public class GameManager : MonoBehaviour
    {
        private List<IRiddle> Riddles = new List<IRiddle>();
        public List<GameObject> ObjectsWithRiddles = new List<GameObject>();
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
                var Riddles = ObjectsWithRiddles
                                .Select(go => go.GetComponent(typeof(IRiddle)))
                                .Select(r => r as IRiddle)
                                .Where(r => r != null)
                                .ToList();
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