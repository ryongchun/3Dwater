using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiUIMgr : MonoBehaviour {

	////////////////////////////////////////////
	//
	// Variable
	//
	////////////////////////////////////////////

	[SerializeField] Button[] m_Buttons;
	
	SimulateViewer	m_SimulateViewer;
	MultiModelMgr	m_MultiModelMgr;


	////////////////////////////////////////////
	//
	// Function
	//
	////////////////////////////////////////////

	private void Awake() {
		m_SimulateViewer	= GameObject.Find("SimulateViewer").GetComponent<SimulateViewer>();
		m_MultiModelMgr		= GameObject.Find("ModelMgr").GetComponent<MultiModelMgr>();
		//
		var singleUIMgr = SingleUIMgr.instance;
		singleUIMgr.fSimulation = this.OnSimulate;
		singleUIMgr.fEndSimulation = this.OnEndSimulate;
	}
	
	public void OnNext() {
		m_MultiModelMgr.MovePage(true);
	}

	public void OnPrev() {
		m_MultiModelMgr.MovePage(false);
	}

	public void OnAnimation() {
		SingleUIMgr.instance.enableSimulateButton = false;
		// Disable Prev&Next Button
		for (int i = 0; i < 2; ++i) {
			m_Buttons[i].interactable = false;
		}
		m_MultiModelMgr.OnAnimation();
	}

	public void OnStop() {
		SingleUIMgr.instance.enableSimulateButton = true;
		// Enable Prev&Next Button
		for (int i = 0; i < 2; ++i) {
			m_Buttons[i].interactable = true;
		}
		m_MultiModelMgr.OnStop();
	}

	void OnSimulate() {
		for(int i = 0; i<m_Buttons.Length; ++i) {
			m_Buttons[i].interactable = false;
		}
	}

	void OnEndSimulate(SimulateViewer simulateViewer) {
		for (int i = 0; i < m_Buttons.Length; ++i) {
			m_Buttons[i].interactable = true;
		}
	}

}
