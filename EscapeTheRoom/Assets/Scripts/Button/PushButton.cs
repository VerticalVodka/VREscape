using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VREscape
{
    public class PushButton : Button
    {
        private Transform bumper;

        private float Scale;

        // Use this for initialization
        void Start()
        {
            base.Start();
            bumper = transform.Find("bumper");
            Scale = bumper.transform.position.y / 5;
        }

        protected override void ButtonDown()
        {
            bumper.Translate(bumper.transform.up * -Scale);
            //HwManager.SendValue(Enums.UnlockEnum.Safe);
            base.ButtonDown();
        }

        protected override void ButtonUp()
        {
            bumper.Translate(bumper.transform.up * Scale);
            base.ButtonDown();
        }
    }
}