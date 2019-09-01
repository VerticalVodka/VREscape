using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace VREscape
{
	public class RiddleOutro : MonoBehaviour, IRiddle {
		
		public GameObject RoomBox;
		public GameObject[] Walls;
		public event Action<bool> OnRiddleDone;
		public float FallDuration = 5.0f;

        public AudioSource OutroAudioSource;
        public AudioClip OutroAudioClip;
		
		private bool isRotating = false;
		private Vector3[] positions = new Vector3[4];

		// Use this for initialization
		void Start () {
			for(int i = 0; i<4; ++i){
				positions[i] = new Vector3(Walls[i].transform.position.x, Walls[i].transform.position.y - Walls[i].transform.lossyScale.y / 2, Walls[i].transform.position.z);
			}
		}
	
		// Update is called once per frame
		void Update () {
		}
		
        public void StartRiddle()
        {
			RoomBox.SetActive(false);
			foreach(var wall in Walls){
				wall.SetActive(true);
			}

            if (OutroAudioClip != null)
            {
                OutroAudioSource?.PlayOneShot(OutroAudioClip);
            }
            
			StartCoroutine(DestroyRoom());
        }

        public void SkipRiddle()
        {
            Debug.Log("Skipped Outro Riddle");
            FinishLevel();
        }
		
		IEnumerator DestroyRoom(){
			isRotating = true;
			for(float j = 0.0f; j<180; ++j){
				for(int i = 0; i<4; ++i){
					Walls[i].transform.RotateAround(positions[i], Walls[i].transform.right, j/180);
				}
				yield return new WaitForSeconds(FallDuration/180);
			}
			FinishLevel();
		}
		
		public void FinishLevel(){
			OnRiddleDone?.Invoke(true);
		}
	}
}
