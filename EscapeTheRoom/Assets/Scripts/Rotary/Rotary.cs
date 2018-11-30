using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VREscape
{
    [RequireComponent(typeof(Renderer))]
    class Rotary : MonoBehaviour
    {
        private AudioSource _audioSource;
        private Renderer _renderer;
        private HWManager _hwManager;

        protected bool flips; // if MaxValue++ becomes MinValue
        protected int currentState;

        public AudioClip RadioTurnSound;
        public Enums.RotaryEnum RotaryType;
        public int StepSize;
        public int MinValue;
        public int MaxValue;
        public int StartValue;

        protected void Start()
        {
            _hwManager = FindObjectOfType<HWManager>();
            _audioSource = FindObjectOfType<AudioSource>();
            _renderer = GetComponent<Renderer>();
            currentState = StartValue;
        }

        protected virtual void RotaryTurned()
        {
            _audioSource?.PlayOneShot(RadioTurnSound);
        }

        private void Update()
        {
            int newRotaryState = _hwManager.GetRotaryState(RotaryType);
            if (newRotaryState != currentState)
            {
                int rotation = newRotaryState - currentState;
                gameObject.transform.Rotate(Vector3.forward, (360 / StepSize * rotation));
                currentState = newRotaryState;
            }
        }

    }
}
