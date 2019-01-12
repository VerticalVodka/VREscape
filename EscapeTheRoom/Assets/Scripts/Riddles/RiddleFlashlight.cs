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

        private bool active = false;
        private FlashLight FlashLight;

        private HWManager hwManager;

        public void StartRiddle()
        {
            hwManager = FindObjectOfType<HWManager>();
            FlashLight = FindObjectOfType<FlashLight>();
            active = true;
            Debug.Log("RiddleFlashlight started");
            StartCoroutine(Do());
        }

        private void Update()
        {
            if (active)
            {
                RaycastHit hit;
                LayerMask mask = LayerMask.GetMask("Letter");

                Ray ray = new Ray(FlashLight.transform.position, -FlashLight.transform.right);

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
                {
                    var mr = hit.transform.gameObject.GetComponent<MeshRenderer>();
                    mr.enabled = true;
                }
            }
        }

        private IEnumerator Do()
        {
            foreach (var lightSource in FindObjectOfType<Room>().GetComponentsInChildren<Light>())
            {
                lightSource.enabled = false;
            }

           FlashLight.EnableLight();

            while (!hwManager.GetButtonState(Enums.ButtonEnum.Button5))
            {
                yield return new WaitForSecondsRealtime(0);
            }

            foreach (var lightSource in FindObjectOfType<Room>().GetComponentsInChildren<Light>())
            {
                lightSource.enabled = true;
            }

            FlashLight.DisableLight();

            
            Debug.Log("RiddleFlashlight solved");
            OnRiddleDone?.Invoke(true);
            active = false;
        }
    }
}