﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VREscape
{
    public class PushButton : Button
    {
        private Transform bumper;

        public float Scale;

        // Use this for initialization
        void Start()
        {
            base.Start();
            bumper = transform.Find("bumper");
            Scale = bumper.transform.position.y / 2;
        }

        protected override void ButtonDown()
        {
            bumper.Translate(bumper.transform.up * Scale);
            //HwManager.SendValue(Enums.UnlockEnum.Drawer);
            base.ButtonDown();
        }

        protected override void ButtonUp()
        {
            bumper.Translate(bumper.transform.up * -Scale);
            base.ButtonDown();
        }
    }
}