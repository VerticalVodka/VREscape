using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioRandomBackgroundNoise : MonoBehaviour {
	public AudioSource AudioSource;
	public List<AudioClip> AudioOptions = new List<AudioClip>();
	public List<float> PauseOptions = new List<float>();


	private System.Random random = new System.Random();
	private bool isRunning = false;
	
	public void StartNoise() {
		isRunning = true;
		StartCoroutine(BackgroundNoise());
	}
	
	public void StopNoise() {
		isRunning = false;
	}
	
	private IEnumerator BackgroundNoise() {
		while(isRunning) {
			var randomIdx = random.Next(AudioOptions.Count + PauseOptions.Count);
			
			if (randomIdx < AudioOptions.Count) {
                AudioSource.PlayOneShot(AudioOptions[randomIdx]);
				yield return new WaitForSecondsRealtime(AudioOptions[randomIdx].length);
			} else {
				randomIdx -= AudioOptions.Count;
				
				yield return new WaitForSecondsRealtime(PauseOptions[randomIdx]);
			}
		}
	}
}
