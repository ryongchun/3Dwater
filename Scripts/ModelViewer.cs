using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelViewer : MonoSingleton<ModelViewer> {

	////////////////////////////////////////////
	//
	// Property
	//
	////////////////////////////////////////////

	public bool isShow { get { return gameObject.activeSelf; } }


	////////////////////////////////////////////
	//
	// Function
	//
	////////////////////////////////////////////

	public void Show(GameObject model) {
		gameObject.SetActive(true);
		GetComponent<MeshFilter>().sharedMesh = model.GetComponent<MeshFilter>().sharedMesh;
		transform.position = model.transform.position;
		transform.rotation = model.transform.rotation;
		transform.localScale = model.transform.localScale;
	}

	public void Hide() {
		gameObject.SetActive(false);
	}
}
