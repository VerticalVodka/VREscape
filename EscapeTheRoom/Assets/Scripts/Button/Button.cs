using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VREscape;

namespace VREscape
{
    [RequireComponent(typeof(AudioSource))]
    public class Button : MonoBehaviour
    {
        private AudioSource _audioSource;
        protected HWManager HwManager;

        public AudioClip ButtonClickSound;
        public AudioClip ButtonReleaseSound;
        public Enums.ButtonEnum ButtonType;

        public bool IsPressed { get; private set; } = false;

        // Use this for initialization
        protected void Start()
        {
            HwManager = FindObjectOfType<HWManager>();
            _audioSource = FindObjectOfType<AudioSource>();
        }

        // Update is called once per frame
        void Update()
        {
            if (HwManager.GetButtonState(ButtonType))
            {
                if (!IsPressed)
                {
                    ButtonDown();
                }

                IsPressed = true;
            }
            else
            {
                if (IsPressed)
                {
                    ButtonUp();
                }

                IsPressed = false;
            }
        }

        protected virtual void ButtonDown()
        {
            if (_audioSource != null)
            {
                _audioSource.PlayOneShot(ButtonClickSound);
            }
        }

        protected virtual void ButtonUp()
        {
            if (_audioSource != null)
            {
                _audioSource.PlayOneShot(ButtonReleaseSound);
            }
        }
    }
}