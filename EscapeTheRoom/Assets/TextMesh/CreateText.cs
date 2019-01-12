using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreateText : MonoBehaviour {

    public GameObject prefab;
    public string text;

    private float cameraRotation;

    // Use this for initialization
    void Start ()
    {

        //Camera.main.transform.position = new Vector3(0, 0.5f, 0);
        //cameraRotation = 0.0f;

        if (text == null)
        {
            text = "Hello World.";
        }

        for (int i = 0; i < text.Length; i++)
        {
            GameObject txtObj = Instantiate(prefab);
            txtObj.transform.Translate(Vector3.right * i * 0.7f);
            //TextMesh txtMesh = (TextMesh) txtObj.GetComponent<TextMesh>();
            //txtMesh.text = text[i].ToString();

            TextMeshPro textmeshPro = txtObj.GetComponent<TextMeshPro>();
            textmeshPro.SetText(text[i].ToString());

            MeshRenderer mr = (MeshRenderer) txtObj.transform.gameObject.GetComponent<MeshRenderer>();
            mr.enabled = false;
        }
	}
	
	void Update () {
        RaycastHit hit;
        LayerMask mask = LayerMask.GetMask("Letter");

        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {
            var mr = hit.transform.gameObject.GetComponent<MeshRenderer>();
            mr.enabled = true;
        }
    }
}
