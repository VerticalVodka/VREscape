﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace VREscape
{
    public class SimonSaysRiddle : MonoBehaviour, IRiddle
    {
        /// <summary>
        /// The amount of successes the users have to have in a row to win the game.
        /// If it is smaller 0, this riddle can't be won through this
        /// </summary>
        public int successesInSequenceForWin = 5;
        /// <summary>
        /// The amount of successes the users have to have overall (with failures in between) to win the game.
        /// If it is smaller 0, this riddle can't be won through this
        /// </summary>
        public int successesTotalForWin = 10;

        [Serializable]
        public struct ButtonAndGameObject
        {
            public Enums.ButtonEnum button;
            public GameObject gameObject;
        }

        /// <summary>
        /// Map of buttons -> GameObjects
        /// The Gameobjects have to have a MeshFilter, whose mesh will get replaced with the respective animal's mesh
        /// </summary>
        public List<ButtonAndGameObject> buttonList;
        public Dictionary<Enums.ButtonEnum, GameObject> buttons;
        /// <summary>
        /// List of animals.
        /// An Animal must have a Component of type mesh which is the mesh that should be shown above the buttons
        ///   and a AudioClip which is the clip telling the player what button to press.
        /// </summary>
        public List<GameObject> animals = new List<GameObject>();
        /// <summary>
        /// The amount of milliseconds to wait before playing all audio-clips again after finishing them
        /// </summary>
        public int audioWaitBetweenSequences;

        /// <summary>
        /// Audiosource which will be playing the animals' sounds
        /// </summary>
        public AudioSource animalAudioSource;
        /// <summary>
        /// Audio source which will be playing the correct and incorrect button sounds
        /// </summary>
        public AudioSource FeedbackAudioSource;
        public AudioClip CorrectButtonAudioClip;
        public AudioClip WrongButtonAudioClip;

        public event Action<bool> OnRiddleDone;

        private HWManager hwManager;

        private bool gameRunning = false;
        private bool isPlayingSounds = false;
        private int successesInSequence = 0;
        private int successesTotal = 0;

        private Dictionary<Enums.ButtonEnum, GameObject> buttonAnimalMap;
        private Enums.ButtonEnum correctButton;
        private Dictionary<Enums.ButtonEnum, bool> buttonStates;


        private void RandomizeButtons()
        {
            if (animals.Count <= 1)
                throw new InvalidOperationException("You need at least two animals to play the SimonSays riddle. Add one to SimonSaysRiddle.animals.");
            while (animals.Count < buttons.Count)
            {
                animals.AddRange(animals);
            }

            var random = new System.Random();
            var randomizedAnimals = animals.OrderBy(x => random.Next()).Take(buttons.Count).GetEnumerator();

            buttonAnimalMap = buttons.ToDictionary(
              (buttonKvp) =>
              {
                  return buttonKvp.Key;
              }, (buttonKvp) =>
              {
                  randomizedAnimals.MoveNext();
                  return randomizedAnimals.Current;
              });
        }

        private void RandomizeSequence()
        {
            if (buttons.Count <= 0)
                throw new InvalidOperationException("You need at least one Button to play the SimonSays riddle. Add one to SimonSaysRiddle.buttons.");

            var random = new System.Random();
            correctButton = buttons.Keys.ElementAt(random.Next(0, buttons.Count));
            sequencePressedIndex = 0;

            Debug.Log("Correct button is " + correctButton);
        }

        private void ShowImagesAboveButtons()
        {
            foreach (var bamkvp in buttonAnimalMap)
            {
                var mashRenderer = buttons[bamkvp.Key].GetComponent<MeshRenderer>();
                mashRenderer.material =
                  bamkvp.Value.GetComponent<MeshRenderer>().sharedMaterial;
                mashRenderer.enabled = true;
            }
        }

        private void StartPlayingSequenceSound()
        {
            sequenceAudioIndex = -1;
            shouldAudioPlay = true;
        }

        private void StopPlayingSequenceSound()
        {
            shouldAudioPlay = false;
            animalAudioSource.Stop();
        }

        private void StopShowingImagesAboveButtons()
        {
            foreach (var bamkvp in buttonAnimalMap)
            {
                buttons[bamkvp.Key].GetComponent<MeshRenderer>().enabled = false;
            }
        }

        public void StartRiddle()
        {
            gameRunning = true;
            lastCorrectButton = null;
            LoadNewLevel();
        }

        private void LoadNewLevel()
        {
            Debug.Log("Loading new Level");
            StartPlayingSequenceSound();
            RandomizeButtons();
            RandomizeSequence();
            ShowImagesAboveButtons();
        }

        private void EndLevel()
        {
            StopPlayingSequenceSound();
            StopShowingImagesAboveButtons();
        }

        private bool IsEndOfGame()
        {
            return successesInSequence == successesInSequenceForWin
              || successesTotal == successesTotalForWin;
        }

        private void WinGame(bool success)
        {
            gameRunning = false;
            OnRiddleDone?.Invoke(success);
        }

        private void WinLevel()
        {
            Debug.Log("Won Level");
            EndLevel();
            ++successesInSequence;
            ++successesTotal;
            if (!IsEndOfGame())
            {
                LoadNewLevel();
            }
            else
            {
                WinGame(true);
            }
        }

        private void ResetLevel()
        {
            Debug.Log("Reset Level");
            EndLevel();
            successesInSequence = 0;
            if (!IsEndOfGame())
            {
                LoadNewLevel();
            }
            else
            {
                WinGame(false);
            }
        }

        private int sequencePressedIndex;

        public void Start()
        {
            hwManager = FindObjectOfType<HWManager>();
            buttons = buttonList.ToDictionary(kvp => kvp.button, kvp => kvp.gameObject);

            buttonStates = new Dictionary<Enums.ButtonEnum, bool>();
            foreach(var en in buttons.Keys) {
                buttonStates.Add(en, hwManager.GetButtonState(en));
            }
        }

        private Nullable<Enums.ButtonEnum> lastCorrectButton;
        private bool isAudioReady = true;
        private bool shouldAudioPlay = false;
        private int sequenceAudioIndex;

        public void Update()
        {
            if (!gameRunning)
                return;
            if (shouldAudioPlay && isAudioReady)
            {
                var clip = buttonAnimalMap[correctButton].GetComponent<AudioSource>().clip;
                animalAudioSource.PlayOneShot(clip);

                isAudioReady = false;
                int waitTime = audioWaitBetweenSequences + (int)(clip.length * 1000);
                Task waitForClipFinished = new Task(async () =>
                {
                    await Task.Delay(waitTime);
                    isAudioReady = true;
                });
                waitForClipFinished.Start();
            }

            if (AnyWrongButtonGotPressed())
            {
                PlayWrongButtonSound();
                ResetLevel();
                return;
            }

            if (CorrectButtonGotPressed())
            {
                PlayCorrectButtonSound();
                WinLevel();
                return;
            }
        }

        private bool AnyWrongButtonGotPressed()
        {
            bool wrongBtnGotPressed = false;
            foreach(var en in buttons.Keys)
            {
                if (en != correctButton
                    && hwManager.GetButtonState(en) != buttonStates[en])
                {
                    buttonStates[en] = hwManager.GetButtonState(en);
                    if (buttonStates[en])
                        wrongBtnGotPressed = true;
                }
            }
            return wrongBtnGotPressed;
        }

        private bool CorrectButtonGotPressed()
        {
            if (hwManager.GetButtonState(correctButton) != buttonStates[correctButton])
            {
                buttonStates[correctButton] = hwManager.GetButtonState(correctButton);
                return buttonStates[correctButton];
            }
            return false;
        }

        private void PlayWrongButtonSound()
        {
            FeedbackAudioSource.PlayOneShot(WrongButtonAudioClip);
        }

        private void PlayCorrectButtonSound()
        {
            FeedbackAudioSource.PlayOneShot(CorrectButtonAudioClip);
        }
    }
}
