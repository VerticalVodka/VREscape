using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VREscape
{
  public class SimonSaysRiddle : MonoBehaviour, IRiddle
  {
    /// <summary>
    /// Amount of sounds in a row for one level
    /// </summary>
    public int sequenceSize = 1;
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

    /// <summary>
    /// Map of buttons -> GameObjects
    /// The Gameobjects have to have a MeshFilter, whose mesh will get replaced with the respective animal's mesh
    /// </summary>
    public Dictionary<Enums.ButtonEnum, GameObject> buttons;
    /// <summary>
    /// List of animals.
    /// An Animal must have a Component of type mesh which is the mesh that should be shown above the buttons
    ///   and a AudioClip which is the clip telling the player what button to press.
    /// </summary>
    public List<GameObject> animals = new List<GameObject>();
    /// <summary>
    /// The amount of seconds to wait before playing all audio-clips again after finishing them
    /// </summary>
    public int audioWaitBetweenSequences;

    /// <summary>
    /// Audiosource which will be playing the sounds
    /// </summary>
    public AudioSource audioSource;

    public event Action<bool> OnRiddleDone;

    private HWManager hwManager;

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

    void RandomizeSequence()
    {
      if (buttons.Count <= 0)
        throw new InvalidOperationException("You need at least one Button to play the SimonSays riddle. Add one to SimonSaysRiddle.buttons.");

      var random = new System.Random();
      currentSequence = new List<Enums.ButtonEnum>();
      for (int i = 0; i < sequenceSize; ++i)
      {
        currentSequence.Add(buttons.Keys.ElementAt(random.Next(0, buttons.Count - 1)));
      }
    }

    void ShowImagesAboveButtons()
    {
      foreach (var bamkvp in buttonAnimalMap)
      {
        buttons[bamkvp.Key].GetComponent<MeshFilter>().mesh = bamkvp.Value.GetComponent<Mesh>();
      }
    }

    void StartPlayingSequenceSound()
    {
      isPlayingSounds = true;
      sequenceAudioIndex = -1;
    }

    void StopPlayingSequenceSound()
    {
      isPlayingSounds = false;
    }

    void StopShowingImagesAboveButtons()
    {
      foreach (var bamkvp in buttonAnimalMap)
      {
        buttons[bamkvp.Key].GetComponent<MeshFilter>().mesh = null;
      }
    }

    public void StartRiddle()
    {
      LoadNewLevel();
    }

    private void LoadNewLevel()
    {
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
      OnRiddleDone?.Invoke(success);
    }

    private void WinLevel()
    {
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

    private int sequencePressedIndex = 0;
    public void Start()
    {
      hwManager = FindObjectOfType<HWManager>();
    }

    public void Update()
    {
      foreach( var btnKvp in buttons)
      {
        if (btnKvp.Key != currentSequence[sequencePressedIndex])
        {
          ResetLevel();
          return;
        }
      }
      if(hwManager.GetButtonState(currentSequence[sequencePressedIndex]))
        ++sequencePressedIndex;

      if (sequencePressedIndex > currentSequence.Count)
      {
        WinLevel();
      }

      if (isPlayingSounds)
        StartCoroutine(PlaySounds());
    }

    private int sequenceAudioIndex = -1;

    private IEnumerator<WaitForSeconds> PlaySounds()
    {
      while (true)
      {
        ++sequenceAudioIndex;
        if (sequenceAudioIndex > currentSequence.Count)
        {
          sequenceAudioIndex = 0;
          yield return new WaitForSeconds(audioWaitBetweenSequences);
        }
        AudioClip clip = buttonAnimalMap[currentSequence[sequenceAudioIndex]].GetComponent<AudioClip>();
        audioSource.PlayOneShot(clip);
        yield return new WaitForSeconds(clip.length);
      }
    }
  }
}
