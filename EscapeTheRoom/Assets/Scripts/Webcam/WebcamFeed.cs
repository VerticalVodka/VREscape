using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class WebcamFeed : MonoBehaviour {
    WebCamTexture _webcamtex;
    // Use this for initialization
    void Start () {

        WebCamDevice[] devices = WebCamTexture.devices;
        /*
         * for (int i = 0; i < devices.Length; i++)
         *  Debug.Log(devices[i].name);
         */
                                       //Hardcoded webcam name 
        _webcamtex = new WebCamTexture("Live! Cam Connect HD VF0750");
        Renderer _renderer = GetComponent<Renderer>();
        _renderer.material.mainTexture = _webcamtex;
        _webcamtex.Play();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
