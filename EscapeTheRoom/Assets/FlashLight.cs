using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLight : MonoBehaviour
{
	private Light light;
	
	// Use this for initialization
	void Start ()
	{
		light = GetComponentInChildren<Light>();
		light.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void EnableLight()
	{
		light.enabled = true;
	}
}
