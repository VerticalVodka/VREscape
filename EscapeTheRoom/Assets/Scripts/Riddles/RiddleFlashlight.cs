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

        public bool active = false;
        private FlashLight FlashLight;

        public void StartRiddle()
        {
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
            foreach (var light in FindObjectOfType<Room>().GetComponentsInChildren<Light>())
            {
                light.enabled = false;
            }

           FlashLight.EnableLight();

            while (true)
            {
                yield return new WaitForSecondsRealtime(0);
            }

            Debug.Log("RiddleFlashlight solved");
            OnRiddleDone?.Invoke(true);
            active = false;
        }
    }
}