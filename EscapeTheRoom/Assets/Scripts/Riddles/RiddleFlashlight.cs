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
        private bool buttonPressed = false;
        private FlashLight FlashLight;
        private Drawer drawer;

        private HWManager hwManager;

        public void StartRiddle()
        {
            drawer = FindObjectOfType<Drawer>();
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

                if (FlashLight == null)
                {
                    FlashLight = FindObjectOfType<FlashLight>();
                }

                Ray ray = new Ray(FlashLight.transform.position, -FlashLight.transform.right);

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
                {
                    var mr = hit.transform.gameObject.GetComponent<MeshRenderer>();
                    mr.enabled = true;
                }

                if (hwManager.GetButtonState(Enums.ButtonEnum.Button5))
                {
                    buttonPressed = true;
                }
            }
        }

        private IEnumerator Do()
        {
            hwManager.SendValue(Enums.UnlockEnum.Drawer);
            drawer.Open();
            foreach (var lightSource in FindObjectOfType<Room>().GetComponentsInChildren<Light>())
            {
                lightSource.enabled = false;
            }

            FlashLight.EnableLight();

            while (!buttonPressed)
            {
                yield return new WaitForSecondsRealtime(0);
            }

            foreach (var lightSource in FindObjectOfType<Room>().GetComponentsInChildren<Light>())
            {
                lightSource.enabled = true;
            }

            FlashLight.DisableLight();
            drawer.Close();


            Debug.Log("RiddleFlashlight solved");
            OnRiddleDone?.Invoke(true);
            active = false;
        }
    }
}