using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLight : MonoBehaviour
{
	private Light light;
	
	void Start ()
	{
		light = GetComponentInChildren<Light>();
		light.enabled = false;
	}
	
	void Update () {
		
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
