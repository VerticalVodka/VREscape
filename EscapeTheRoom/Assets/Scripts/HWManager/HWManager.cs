using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SerialTest;
using UnityEngine;
using Extensions;

namespace VREscape
{
    public class HWManager : MonoBehaviour
    {
        private Dictionary<Enums.ButtonEnum, bool> _buttons;
        private Dictionary<Enums.RotaryEnum, int> _rotaries;
        public bool TestMode = true;
        public string COMPort = "COM9";
        private InputQueue _inputQueue;

        public int UpdatesPerSecond = 5;

        private float deltaTime;
        private float interpolatedTime = 0.0f;


        public void Awake()
        {
            Application.targetFrameRate = 100;
            deltaTime = 1.0f / (float)UpdatesPerSecond;
            _inputQueue = new InputQueue(COMPort);

            _buttons = new Dictionary<Enums.ButtonEnum, bool>
            {
                {Enums.ButtonEnum.Button1, false},
                {Enums.ButtonEnum.Button2, false},
                {Enums.ButtonEnum.Button3, false},
                {Enums.ButtonEnum.Button4, false},
                {Enums.ButtonEnum.Button5, false},
                {Enums.ButtonEnum.Rotary1, false},
                {Enums.ButtonEnum.Rotary2, false}
            };
            _rotaries = new Dictionary<Enums.RotaryEnum, int>()
            {
                {Enums.RotaryEnum.Rotary1, 0},
                {Enums.RotaryEnum.Rotary2, 0}
            };
            _inputQueue.StartListening();
        }

        private void OnDestroy()
        {
            _inputQueue.StopListening();
			Debug.Log("Disposed correctly");
		}

        public bool GetButtonState(Enums.ButtonEnum button)
        {
            return _buttons[button];
        }

        public int GetRotaryState(Enums.RotaryEnum rotary)
        {
            return _rotaries[rotary];
        }

        public void SendValue(Enums.UnlockEnum unlockEnum)
        {
            _inputQueue.SendData(unlockEnum.GetDescription());
        }

        public void Update()
        {
            deltaTime += Time.deltaTime;

            if (deltaTime >= interpolatedTime)
            {
                deltaTime = 0.0f;
                UpdateInputs();
            }
            UpdateKeyboardInputs();
        }

        private void UpdateInputs()
        {
            _inputQueue.ProcessData(out _buttons, out _rotaries);
        }

        private void UpdateKeyboardInputs()
        {
            if (!TestMode) return;
            if (Input.GetKey(KeyCode.Alpha1))
                _buttons[Enums.ButtonEnum.Button1] = true;
            if (Input.GetKey(KeyCode.Alpha2))
                _buttons[Enums.ButtonEnum.Button2] = true;
            if (Input.GetKey(KeyCode.Alpha3))
                _buttons[Enums.ButtonEnum.Button3] = true;
            if (Input.GetKey(KeyCode.Alpha4))
                _buttons[Enums.ButtonEnum.Button4] = true;
            if (Input.GetKey(KeyCode.K))
            {
                Debug.Log("BUTTON 5");
                _buttons[Enums.ButtonEnum.Button5] = true;
            }

            _rotaries[Enums.RotaryEnum.Rotary1] += Input.GetKeyDown(KeyCode.RightArrow) ? 1 : 0;
            _rotaries[Enums.RotaryEnum.Rotary1] += Input.GetKeyDown(KeyCode.LeftArrow) ? -1 : 0;
            if (Input.GetKey(KeyCode.Alpha5))
                _buttons[Enums.ButtonEnum.Rotary1] = true;
            _rotaries[Enums.RotaryEnum.Rotary2] += Input.GetKey(KeyCode.D) ? 1 : 0;
            _rotaries[Enums.RotaryEnum.Rotary2] += Input.GetKey(KeyCode.A) ? -1 : 0;
            if (Input.GetKey(KeyCode.Alpha6))
                _buttons[Enums.ButtonEnum.Rotary1] = true;
        }
    }
}