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
        private const bool TestMode = true;
        private readonly InputQueue _inputQueue = new InputQueue("COM5");

        public int UpdatesPerSecond = 10;

        private float deltaTime;
        private float interpolatedTime = 0.0f;


        public void Awake()
        {
            deltaTime = 1.0f / UpdatesPerSecond;
            
            _buttons = new Dictionary<Enums.ButtonEnum, bool>
            {
                {Enums.ButtonEnum.Button1, false},
                {Enums.ButtonEnum.Button2, false},
                {Enums.ButtonEnum.Button3, false},
                {Enums.ButtonEnum.Button4, false},
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
 
            if (deltaTime >= interpolatedTime) {
                deltaTime = 0.0f;
                UpdateInputs();
            }
            
        }

        private void UpdateInputs()
        {
            _inputQueue.ProcessData(out _buttons, out _rotaries);
            if (TestMode) return;
            _buttons[Enums.ButtonEnum.Button1] = Input.GetKey(KeyCode.Alpha1);
            _buttons[Enums.ButtonEnum.Button2] = Input.GetKey(KeyCode.Alpha2);
            _buttons[Enums.ButtonEnum.Button3] = Input.GetKey(KeyCode.Alpha3);
            _buttons[Enums.ButtonEnum.Button4] = Input.GetKey(KeyCode.Alpha4);
            _rotaries[Enums.RotaryEnum.Rotary1] += Input.GetKey(KeyCode.LeftArrow) ? 1 : 0;
            _rotaries[Enums.RotaryEnum.Rotary1] += Input.GetKey(KeyCode.RightArrow) ? -1 : 0;
            _buttons[Enums.ButtonEnum.Rotary1] = Input.GetKey(KeyCode.DownArrow);
            _rotaries[Enums.RotaryEnum.Rotary1] += Input.GetKey(KeyCode.A) ? 1 : 0;
            _rotaries[Enums.RotaryEnum.Rotary1] += Input.GetKey(KeyCode.D) ? -1 : 0;
            _buttons[Enums.ButtonEnum.Rotary1] = Input.GetKey(KeyCode.S);
        }
    }
}