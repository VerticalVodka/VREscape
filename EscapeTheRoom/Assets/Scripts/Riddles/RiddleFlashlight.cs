using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VREscape
{
    class RiddleFlashlight : MonoBehaviour, IRiddle
    {
        public event Action<bool> OnRiddleDone;

        public GameObject LookGO;
        public GameObject WhereGO;
        public GameObject TheGO;
        public GameObject LightGO;
        public GameObject CameGO;
        public GameObject FromGO;

        public AudioClip LookAudioFW;
        public AudioClip WhereAudioFW;
        public AudioClip TheAudioFW;
        public AudioClip LightAudioFW;
        public AudioClip CameAudioFW;
        public AudioClip FromAudioFW;

        public AudioClip LookAudioBW;
        public AudioClip WhereAudioBW;
        public AudioClip TheAudioBW;
        public AudioClip LightAudioBW;
        public AudioClip CameAudioBW;
        public AudioClip FromAudioBW;

        private List<WordBundle> wordBundles;
        private IEnumerator<WordBundle> wordBundleEnumerator;

        private bool active = false;
        public bool Dark => !active;
        private bool buttonPressed = false;
        public FlashLight FlashLight;
        private Drawer drawer;

        private HWManager hwManager;


        void Start()
        {
            drawer = FindObjectOfType<Drawer>();
            hwManager = FindObjectOfType<HWManager>();
            wordBundles = new List<WordBundle>()
            {
                new WordBundle
                {
                    Parent = LookGO,
                    ForwardClip = LookAudioFW,
                    BackwardClip = LookAudioBW
                },
                 new WordBundle
                {
                    Parent = WhereGO,
                    ForwardClip = WhereAudioFW,
                    BackwardClip = WhereAudioBW
                },
                  new WordBundle
                {
                    Parent = TheGO,
                    ForwardClip = TheAudioFW,
                    BackwardClip = TheAudioBW
                },
                   new WordBundle
                {
                    Parent = LightGO,
                    ForwardClip = LightAudioFW,
                    BackwardClip = LightAudioBW
                },
                    new WordBundle
                {
                    Parent = CameGO,
                    ForwardClip = CameAudioFW,
                    BackwardClip = CameAudioBW
                },  new WordBundle
                {
                    Parent = FromGO,
                    ForwardClip = FromAudioFW,
                    BackwardClip = FromAudioBW
                }
            };

        }

        public void StartRiddle()
        {
            active = true;
            FlashLight.MeshEnabled = true;
            Debug.Log("RiddleFlashlight started");
            StartCoroutine(Do());
            StartCoroutine(PlayAudioHints());
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

        private IEnumerator PlayAudioHints()
        {
            while (active)
            {
                foreach (var bundle in wordBundles)
                {
                    var clip = isDiscoverd(bundle.Parent)
                        ? bundle.ForwardClip
                        : bundle.BackwardClip;

                    bundle.AudioSource.PlayOneShot(clip);
                    yield return new WaitForSecondsRealtime(clip.length);
                }
            }
        }

        private void FinishLevel()
        {
            foreach (var lightSource in FindObjectOfType<Room>().GetComponentsInChildren<Light>())
            {
                lightSource.enabled = true;
            }

            GameObject.FindWithTag("hinttext")?.SetActive(false);

            drawer.Close();
            FlashLight.gameObject.SetActive(false);
            OnRiddleDone?.Invoke(true);
            active = false;
        }

        private bool isDiscoverd(GameObject go)
        {
            return go.GetComponentsInChildren<MeshRenderer>().All(m => m.isVisible);
        }

        private class WordBundle
        {
            public GameObject Parent;
            public AudioClip ForwardClip;
            public AudioClip BackwardClip;
            public AudioSource AudioSource {
                get {
                    return Parent.GetComponentInChildren<AudioSource>();
                }
            }
        }
    }
}