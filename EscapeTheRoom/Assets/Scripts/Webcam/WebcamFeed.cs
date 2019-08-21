using UnityEngine;

public class WebcamFeed : MonoBehaviour
{
    public string WebCamName = "Live! Cam Connect HD VF0750";

    private WebCamTexture webcamtex;

    private void Start()
    {	
		WebCamDevice[] devices = WebCamTexture.devices;
        for (int i = 0; i < devices.Length; i++)
            Debug.Log(devices[i].name);
		
        webcamtex = new WebCamTexture(WebCamName);
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = webcamtex;
		webcamtex.Play();
    }

    public void Play()
    {
		
    }
}
