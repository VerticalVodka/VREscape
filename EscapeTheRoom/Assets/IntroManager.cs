using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VREscape;

public class IntroManager : MonoBehaviour {
	
	private HWManager hwmanager;

	// Use this for initialization
	void Start () {
		hwmanager = FindObjectOfType<HWManager>();
	}
	
	// Update is called once per frame
	void Update () {
		if(hwmanager.GetButtonState(Enums.ButtonEnum.Button1)){
			StartGame();
		}
	}
	
	void StartGame(){
		SceneManager.LoadScene("SampleScene");
	
	}
}
