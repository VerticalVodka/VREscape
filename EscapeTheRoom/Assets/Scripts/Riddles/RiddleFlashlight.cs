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
		public bool Dark => !active;
        private bool buttonPressed = false;
        public FlashLight FlashLight;
        private Drawer drawer;

        private HWManager hwManager;
		
			
		void Start(){
				drawer = FindObjectOfType<Drawer>();
				hwManager = FindObjectOfType<HWManager>();
		}

        public void StartRiddle()
        {
            active = true;
            FlashLight.MeshEnabled = true;
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

        public void SkipRiddle()
        {
            Debug.Log("Skipped Flashligh Riddle");
            FinishLevel();
        }

        private IEnumerator Do()
        {
            hwManager.SendValue(Enums.UnlockEnum.Drawer);
            drawer.Open();
            foreach (var lightSource in FindObjectOfType<Room>().GetComponentsInChildren<Light>())
            {
                lightSource.enabled = false;
            }

            while (!buttonPressed)
            {
                yield return new WaitForSecondsRealtime(0);
            }
            
            Debug.Log("RiddleFlashlight solved");
            FinishLevel();
        }

        private void FinishLevel() 
        {
            foreach (var lightSource in FindObjectOfType<Room>().GetComponentsInChildren<Light>())
            {
                lightSource.enabled = true;
            }
			
			GameObject.FindWithTag("hinttext").SetActive(false);
			
            drawer.Close();
			FlashLight.gameObject.SetActive(false);
            OnRiddleDone?.Invoke(true);
            active = false;
        }
    }
}