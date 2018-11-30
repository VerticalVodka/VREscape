using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VREscape
{
    public class RiddleRadio : MonoBehaviour, IRiddle
    {
        private HWManager hWManager;

        public event Action<bool> OnRiddleDone;

        public void StartRiddle()
        {
            Debug.Log("RiddleRadio started");
            hWManager = FindObjectOfType<HWManager>();
            StartCoroutine(Do());
            if (OnRiddleDone != null)
            {
                OnRiddleDone.Invoke(true);
            }
        }

        IEnumerator Do()
        {
            while (hWManager.GetButtonState(Enums.ButtonEnum.Button1))
                yield return new WaitForSecondsRealtime(0);
            while (hWManager.GetRotaryState(Enums.RotaryEnum.Rotary2) != 10)
                yield return new WaitForSecondsRealtime(0);
        }
    }
}