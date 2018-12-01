using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VREscape
{
    [RequireComponent(typeof(Renderer))]
    public class Rotary : MonoBehaviour
    {
        private AudioSource _audioSource;
        private Renderer _renderer;
        private HWManager _hwManager;

        protected bool flips; // if MaxValue++ becomes MinValue

        public AudioClip RadioTurnSound;
        public Enums.RotaryEnum RotaryType;
        public int RotaryTotalSteps = 20;
        public int MinValue;
        public int MaxValue;
        public int StartValue;
        public int CurrentState;

        protected void Start()
        {
            _hwManager = FindObjectOfType<HWManager>();
            _audioSource = FindObjectOfType<AudioSource>();
            _renderer = GetComponent<Renderer>();
            CurrentState = StartValue;
        }

        protected virtual void RotaryTurned()
        {
            _audioSource?.PlayOneShot(RadioTurnSound);
        }

        private void Update()
        {
            int newRotaryState = _hwManager.GetRotaryState(RotaryType) + CurrentState;
            if (newRotaryState != CurrentState)
            {
                int rotation = newRotaryState - CurrentState;
                gameObject.transform.Rotate(Vector3.forward, (360 / RotaryTotalSteps * rotation));
                CurrentState = newRotaryState;
                if (flips && CurrentState > MaxValue)
                {
                    CurrentState = MinValue + CurrentState - MaxValue;
                }
                else if (flips && CurrentState < MinValue)
                {
                    CurrentState = MaxValue - CurrentState - MinValue;
                }
                else if (!flips && CurrentState > MaxValue)
                {
                    CurrentState = MaxValue;
                }
                else if (!flips && CurrentState < MinValue)
                {
                    CurrentState = MinValue;
                }
            }
        }
    }
}