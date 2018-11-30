﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SerialTest;
using UnityEngine;

namespace VREscape
{
    public class HWManager : MonoBehaviour
    {
        private Dictionary<Enums.ButtonEnum, bool> _buttons;
        private Dictionary<Enums.RotaryEnum, int> _rotaries;
        private readonly InputQueue _inputQueue = new InputQueue("COM5");


        public void Start()
        {
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
        }

        public bool GetButtonState(Enums.ButtonEnum button)
        {
            return _buttons[button];
        }

        public int GetRotaryState(Enums.RotaryEnum rotary)
        {
            return _rotaries[rotary];
        }

        public void Update()
        {
            _inputQueue.ProcessData(out _buttons, out _rotaries);
        }
    }
}