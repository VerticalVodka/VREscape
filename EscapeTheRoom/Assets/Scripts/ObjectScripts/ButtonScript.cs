using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour {

    public HWManager.Button button;
    private bool oldButtonState;

	// Use this for initialization
	void Start () {
        oldButtonState = HWManager.IsButtonPressed(button);
	}
	
	// Update is called once per frame
	void Update () {
        if (HWManager.IsButtonPressed(button) != oldButtonState)
        {
            oldButtonState = !oldButtonState;
            //TODO animation + sound
        }
	}
}
