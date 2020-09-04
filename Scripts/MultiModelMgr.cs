using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiModelMgr : ModelMgr {

	////////////////////////////////////////////
	//
	// Variable
	//
	////////////////////////////////////////////

	[SerializeField] float m_AnimationDelay;

	ModelViewer m_ModelViewer;
	VoxelViewer m_VoxelViewer;
	Page[]		m_Pages;
	int			m_PageIndex;


	////////////////////////////////////////////
	//
	// Property
	//
	////////////////////////////////////////////

	public override Page currentPage { get { return m_Pages[m_PageIndex]; } }


	////////////////////////////////////////////
	//
	// Function
	//
	////////////////////////////////////////////

	private void Awake() {
		m_VoxelViewer = VoxelViewer.instance;
		m_ModelViewer = ModelViewer.instance;
		var informationMgr = InformationMgr.instance;
		var models = GetComponentsInChildren<MeshFilter>(true);
		m_Pages = new Page[models.Length];
		for (int i = 0; i < m_Pages.Length; ++i) {
			var model = models[i].gameObject;
			var voxelData = VoxelizeUtility.instance.ToVoxelData(informationMgr, model);
			m_Pages[i] = new Page(model, voxelData);
			model.SetActive(false);
		}
		m_ModelViewer.Show(m_Pages[m_PageIndex].model);
	}

	public void MovePage(bool moveNext) {
		// Move PageIndex
		if(moveNext) {
			m_PageIndex = Mathf.Min(m_PageIndex + 1, m_Pages.Length - 1);
		}
		else {
			m_PageIndex = Mathf.Max(0, m_PageIndex - 1);
		}
		// Refresh Model&Voxel Viewer
		if(m_ModelViewer.isShow) {
			m_ModelViewer.Show(m_Pages[m_PageIndex].model);
		}
		else {
			m_VoxelViewer.Show(m_Pages[m_PageIndex].voxelData);
		}
	}

	public void OnAnimation() {
		MultiSimulateViewer.instance.Simulate(m_Pages[m_PageIndex].voxelData, this.OnEndDrop);
	}

	public void OnStop() {
		MultiSimulateViewer.instance.Stop();
	}

	void OnEndDrop() {
		StartCoroutine(WaitAnimationDelay());
	}

	IEnumerator WaitAnimationDelay() {
		if (m_AnimationDelay != 0.0f) {
			yield return new WaitForSeconds(m_AnimationDelay);
		}
		MovePage(true);
		OnAnimation();
	}

}
