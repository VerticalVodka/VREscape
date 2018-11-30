using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VREscape {
    public class RotaryScript : MonoBehaviour {

        public int StepNumber;
        public Enums.RotaryEnum Rotary;
        private HWManager hWManager;
        private int oldRotaryState;

        void rotate(int rotation)
        {
            gameObject.transform.Rotate(Vector3.forward, (360 / StepNumber * rotation));
        }

        // Use this for initialization
        void Start() {
            hWManager = FindObjectOfType<HWManager>();
            oldRotaryState = hWManager.GetRotaryState(Rotary);
        }

        // Update is called once per frame
        void Update() {
            int newRotaryState = hWManager.GetRotaryState(Rotary);
            if(newRotaryState != oldRotaryState)
            {
                Debug.Log("asdasd");
                rotate(newRotaryState - oldRotaryState);
            }

        }
    }
}