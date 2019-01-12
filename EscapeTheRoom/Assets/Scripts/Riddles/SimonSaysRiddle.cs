using System;
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
        /// Amount of sounds in a row for one level
        /// Currently fixed to 1
        /// </summary>
        private int sequenceSize = 1;
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
        public int audioWaitAfterClip;

        /// <summary>
        /// Audiosource which will be playing the sounds
        /// </summary>
        public AudioSource audioSource;

        public event Action<bool> OnRiddleDone;

        private HWManager hwManager;

        private bool gameRunning = false;
        private bool isPlayingSounds = false;
        private int successesInSequence = 0;
        private int successesTotal = 0;

        private Dictionary<Enums.ButtonEnum, GameObject> buttonAnimalMap;
        private List<Enums.ButtonEnum> currentSequence;

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
            currentSequence = new List<Enums.ButtonEnum>();
            for (int i = 0; i < sequenceSize; ++i)
            {
                currentSequence.Add(buttons.Keys.ElementAt(random.Next(0, buttons.Count - 1)));
            }
            sequencePressedIndex = 0;

            if (Debug.isDebugBuild)
            {
                var sb = new StringBuilder();
                foreach (var seq in currentSequence)
                {
                    sb.Append(seq);
                    sb.Append(">");
                }
                Debug.Log("Current Sequence: " + sb.ToString());
            }
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
            audioSource.Stop();
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
                ++sequenceAudioIndex;
                if (sequenceAudioIndex >= currentSequence.Count)
                {
                    sequenceAudioIndex = -1;
                    isAudioReady = false;
                    Task waitBeforeSequence = new Task(async () =>
                    {
                        if (audioWaitBetweenSequences - audioWaitAfterClip > 0)
                            await Task.Delay(audioWaitBetweenSequences - audioWaitAfterClip);
                        isAudioReady = true;
                    });
                    waitBeforeSequence.Start();
                }
                else
                {
                    AudioClip clip = buttonAnimalMap[currentSequence[sequenceAudioIndex]].GetComponent<AudioSource>().clip;
                    audioSource.PlayOneShot(clip);
                    isAudioReady = false;
                    int waitTime = audioWaitAfterClip + (int)(clip.length * 1000);
                    Task waitForClipFinished = new Task(async () =>
                    {
                        await Task.Delay(waitTime);
                        isAudioReady = true;
                    });
                    waitForClipFinished.Start();
                }
            }

            if (lastCorrectButton.HasValue && !hwManager.GetButtonState(lastCorrectButton.Value))
                lastCorrectButton = null;

            if (!lastCorrectButton.HasValue)
            {
                if (hwManager.GetButtonState(currentSequence[sequencePressedIndex]))
                {
                    lastCorrectButton = currentSequence[sequencePressedIndex];
                    ++sequencePressedIndex;
                }

                if (sequencePressedIndex >= currentSequence.Count)
                {
                    WinLevel();
                    return;
                }

                foreach (var btnKvp in buttons)
                {
                    if (btnKvp.Key != currentSequence[sequencePressedIndex]
                      && hwManager.GetButtonState(btnKvp.Key))
                    {
                        ResetLevel();
                        return;
                    }
                }
            }
        }
    }
}
