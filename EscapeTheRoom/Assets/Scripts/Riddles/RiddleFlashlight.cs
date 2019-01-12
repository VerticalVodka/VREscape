using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VREscape
{
    class RiddleFlashlight : MonoBehaviour, IRiddle
    {
        public event Action<bool> OnRiddleDone;

        public void StartRiddle()
        {
            Debug.Log("RiddleFlashlight started");
            StartCoroutine(Do());
        }

        private IEnumerator Do()
        {
            // TODO: LOGIC
            yield return new WaitForSecondsRealtime(0);

            Debug.Log("RiddleFlashlight solved");
            OnRiddleDone?.Invoke(true);
        }
    }
}
