using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VREscape
{
    public class ButtonScript : MonoBehaviour
    {

        public Enums.ButtonEnum button;
        private bool oldButtonState;
        private HWManager hWManager;

        // Use this for initialization
        void Start()
        {
            hWManager = FindObjectOfType<HWManager>();
            oldButtonState = hWManager.GetButtonState(button);
        }

        // Update is called once per frame
        void Update()
        {
            if (hWManager.GetButtonState(button) != oldButtonState)
            {
                oldButtonState = !oldButtonState;
                //TODO animation + sound
            }
        }
    }
}
