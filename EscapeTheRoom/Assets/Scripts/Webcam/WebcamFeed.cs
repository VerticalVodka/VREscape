using UnityEngine;

public class WebcamFeed : MonoBehaviour
{
    public string WebCamName = "Live! Cam Connect HD VF0750";

    private WebCamTexture webcamtex;

    private void Start()
    {
        webcamtex = new WebCamTexture(WebCamName);
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = webcamtex;
    }

    public void Play()
    {
        webcamtex.Play();
    }
}
