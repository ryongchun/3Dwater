using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleModelMgr : ModelMgr {

	////////////////////////////////////////////
	//
	// Variable
	//
	////////////////////////////////////////////

	Page m_Page;


	////////////////////////////////////////////
	//
	// Property
	//
	////////////////////////////////////////////

	public override Page currentPage { get { return m_Page; } }


	////////////////////////////////////////////
	//
	// Function
	//
	////////////////////////////////////////////

	void Awake() {
		var informationMgr = InformationMgr.instance;
		var model = gameObject;
		var voxelData = VoxelizeUtility.instance.ToVoxelData(informationMgr, model);
		m_Page = new Page(model, voxelData);
		model.SetActive(false);
		ModelViewer.instance.Show(m_Page.model);
	}

}
