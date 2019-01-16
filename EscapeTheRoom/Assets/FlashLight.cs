using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VREscape;

public class FlashLight : MonoBehaviour
{
	private Light light;

	private GameObject parent;

	private SteamVR_TrackedObject controller;
	private RiddleFlashlight riddleFlashlight;

	private RiddleFlashlight RiddleFlashlight
	{
		get
		{
			if (riddleFlashlight == null)
			{
				riddleFlashlight = FindObjectOfType<RiddleFlashlight>();
			}
			return riddleFlashlight;
		}
	}

	void Start ()
	{
		light = GetComponentInChildren<Light>();
		light.enabled = false;
		parent = this.parent;
		controller = parent.GetComponentInChildren<SteamVR_TrackedObject>();
	}
	
	void Update () {
		if (riddleFlashlight.Dark)
		{
			light.enabled = false;
		}
		if (controller != null)
		{
			transform.position = controller.transform.position;
			transform.rotation = controller.transform.rotation;
		}
	}

	public void EnableLight()
	{
		light.enabled = true;
	}

	public void DisableLight()
	{
		light.enabled = false;
	}
}
