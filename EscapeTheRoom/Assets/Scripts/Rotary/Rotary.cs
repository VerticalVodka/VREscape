using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VREscape
{
    public class Rotary : MonoBehaviour
    {
        protected AudioSource _audioSource;
        private Renderer _renderer;
        private HWManager _hwManager;

        protected bool flips; // if MaxValue++ becomes MinValue

        public AudioClip RotaryTurnSound;
        public Enums.RotaryEnum RotaryType;
        public int RotaryTotalSteps = 20;
        public int MinValue;
        public int MaxValue;
        public int StartValue;
        public int CurrentState;

        public virtual void Start()
        {
            _hwManager = FindObjectOfType<HWManager>();
            _audioSource = GetComponent<AudioSource>();
            _renderer = GetComponent<Renderer>();
            CurrentState = StartValue;
            _audioSource.clip = RotaryTurnSound;
        }

        protected virtual void RotaryTurned()
        {
            _audioSource?.PlayOneShot(RotaryTurnSound);
        }

        public virtual void Update()
        {
            int newRotaryState = _hwManager.GetRotaryState(RotaryType) + CurrentState;
            if (newRotaryState != CurrentState)
            {
                int rotation = newRotaryState - CurrentState;
                gameObject.transform.Rotate(Vector3.up, (360 / RotaryTotalSteps * rotation));
                CurrentState = newRotaryState;
                if (flips && CurrentState > MaxValue)
                {
                    CurrentState = MinValue + (CurrentState - MaxValue - 1);
                    Debug.Log(MinValue);
                    Debug.Log(MaxValue);
                    Debug.Log(CurrentState);
                }
                else if (flips && CurrentState < MinValue)
                {
                    CurrentState = MaxValue - (CurrentState - MinValue + 1);
                    Debug.Log(MinValue);
                    Debug.Log(MaxValue);
                    Debug.Log(CurrentState);
                }
                else if (!flips && CurrentState > MaxValue)
                {
                    CurrentState = MaxValue;
                }
                else if (!flips && CurrentState < MinValue)
                {
                    CurrentState = MinValue;
                }
                RotaryTurned();
            }
        }
    }
}