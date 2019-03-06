﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VREscape;

public class FlashLight : MonoBehaviour
{
	private Light[] lights;

	private GameObject parent;

	private SteamVR_TrackedObject controller;
	
	private RiddleFlashlight riddleFlashLight;
	
	private RiddleFlashlight RiddleFlashLight{
	get{
		if(riddleFlashLight == null){
			riddleFlashLight = FindObjectOfType<RiddleFlashlight>();
		}
		return riddleFlashLight;
	}}
	
	void Start ()
	{
		lights = GetComponentsInChildren<Light>();
		
	}
	
	void Update () {
		foreach(var light in lights){
			light.enabled = !RiddleFlashLight.Dark;
		}
	}


}
