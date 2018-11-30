using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiddleRadio : MonoBehaviour, IRiddle {
    public event Action<bool> OnRiddleDone;

    public void StartRiddle()
    {
        Debug.Log("RiddleRadio started");
        StartCoroutine(Do());
        if(OnRiddleDone != null)
        {
            OnRiddleDone.Invoke(true);
        }
    }

    IEnumerator Do()
    {
        while (HWManager.IsButtonPressed(HWManager.Button.button1))
            yield return new WaitForSecondsRealtime(0);
        while (HWManager.IsButtonPressed(HWManager.Button.button2))
            yield return new WaitForSecondsRealtime(0);
    }
}
