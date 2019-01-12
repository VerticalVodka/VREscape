using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class GlowLetter : MonoBehaviour
{
	private TextMeshPro textMeshPro;
	
	// Use this for initialization
	void Start ()
	{
		textMeshPro = GetComponent<TextMeshPro>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void GlowUp()
	{
	}
}
