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
		
		private bool isRotating = false;

		// Use this for initialization
		void Start () {
            StartRiddle();
		}
	
		// Update is called once per frame
		void Update () {
			while(isRotating){
				foreach(var wall in Walls){
					wall.transform.RotateAround(new Vector3(wall.transform.position.x, 0, wall.transform.position.z), Vector3.right, Time.deltaTime * 10);
				}
			}
		}
		
        public void StartRiddle()
        {
			RoomBox.SetActive(false);
			foreach(var wall in Walls){
				wall.SetActive(true);
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
			yield return new WaitForSeconds(15);
			FinishLevel();
		}
		
		public void FinishLevel(){
			OnRiddleDone?.Invoke(true);
		}
	}
}
