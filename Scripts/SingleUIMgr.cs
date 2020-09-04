using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SingleUIMgr : MonoSingleton<SingleUIMgr> {

	////////////////////////////////////////////
	//
	// Variable
	//
	////////////////////////////////////////////

	[SerializeField] Button m_BtnSimulate;

	ModelViewer				m_ModelViewer;
	VoxelViewer				m_VoxelViewer;
	SimulateViewer			m_SimulateViewer;
	ModelMgr				m_ModelMgr;
	Action					m_fSimulation;
	Action<SimulateViewer>	m_fEndSimulation;


	////////////////////////////////////////////
	//
	// Property
	//
	////////////////////////////////////////////

	public Action fSimulation { set { m_fSimulation = value; } }
	public Action<SimulateViewer> fEndSimulation { set { m_fEndSimulation = value; } }
	public bool enableSimulateButton { set { m_BtnSimulate.interactable = value; } }


	////////////////////////////////////////////
	//
	// Function
	//
	////////////////////////////////////////////

	protected override void Awake() {
		base.Awake();
		m_ModelViewer		= ModelViewer.instance;
		m_VoxelViewer		= VoxelViewer.instance;
		m_SimulateViewer	= GameObject.Find("SimulateViewer").GetComponent<SimulateViewer>();
		m_ModelMgr			= GameObject.Find("ModelMgr").GetComponent<ModelMgr>();
	}

	public void SetModel(bool on) {
		if (on == false)
			return;
		m_ModelViewer.Show(m_ModelMgr.currentPage.model);
		m_VoxelViewer.Hide();
	}

	public void SetVoxel(bool on) {
		if (on == false)
			return;
		m_ModelViewer.Hide();
		m_VoxelViewer.Show(m_ModelMgr.currentPage.voxelData);
	}

	public void Simulate() {
		if(m_fSimulation != null) {
			m_fSimulation();
		}
		m_SimulateViewer.Simulate(m_ModelMgr.currentPage.voxelData, m_fEndSimulation);
	}

}
