using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Page {

	////////////////////////////////////////////
	//
	// Variable
	//
	////////////////////////////////////////////

	public GameObject	model;
	public VoxelData	voxelData;


	////////////////////////////////////////////
	//
	// Function
	//
	////////////////////////////////////////////

	public Page(GameObject model, VoxelData voxelData) {
		this.model = model;
		this.voxelData = voxelData;
	}

}
