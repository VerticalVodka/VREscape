using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VREscape
{
    public class GameManager : MonoBehaviour
    {
        private List<IRiddle> Riddles = new List<IRiddle>();
        public List<GameObject> ObjectsWithRiddles = new List<GameObject>();
        private IEnumerator<IRiddle> riddleEnumerator;

        private HWManager hwManager;

        public int PauseBetweenRiddlesInMs = 1000;

        private bool goToNextRiddle = false;

        private void NextRiddle()
        {
            if (riddleEnumerator.MoveNext())
            {
                Task waitBeforeNextRiddle = new Task(async () =>
                {
                    if (PauseBetweenRiddlesInMs > 0)
                    {
                        await Task.Delay(PauseBetweenRiddlesInMs);
                    }

                    goToNextRiddle = true;
                });
                waitBeforeNextRiddle.Start();
            }
            else
            {
                //SceneManager.LoadScene("OutroScene");
            }
        }

        private void OnRiddleDoneListener(bool value)
        {
            riddleEnumerator.Current.OnRiddleDone -= OnRiddleDoneListener;
            NextRiddle();
        }

        private void Start()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
            Time.fixedDeltaTime = 1 / 60;
            hwManager = FindObjectOfType<HWManager>();
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

        private void Update()
        {
            if (goToNextRiddle)
            {
                Debug.Log("Starting the next riddle of type " + riddleEnumerator.Current.GetType());
                goToNextRiddle = false;
                riddleEnumerator.Current.OnRiddleDone += OnRiddleDoneListener;
                riddleEnumerator.Current.StartRiddle();
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                hwManager.SendValue(Enums.UnlockEnum.CloseSafe);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                hwManager.SendValue(Enums.UnlockEnum.Safe);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                hwManager.SendValue(Enums.UnlockEnum.Drawer);
            }
            if (Input.GetKeyDown(KeyCode.L))
            {

                riddleEnumerator.Current?.SkipRiddle();
            }
        }
    }

}