using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VREscape;

public class FlashLight : MonoBehaviour
{
	private Light[] lights;

	private GameObject parent;

	private SteamVR_TrackedObject controller;
	
	private RiddleFlashlight riddleFlashLight;

    private IEnumerable<MeshRenderer> meshRenderers;

    private bool meshEnabled = true;
    public bool MeshEnabled {
        get { return meshEnabled; }
        set {
            foreach(var meshRender in meshRenderers)
            {
                meshRender.enabled = value;
            }
            meshEnabled = value;
        }

    }
	
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
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
	}
	
	void Update () {
		foreach(var light in lights){
			light.enabled = !RiddleFlashLight.Dark;
		}
	}


}
