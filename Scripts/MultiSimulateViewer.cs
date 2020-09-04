using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MultiSimulateViewer : MonoSingleton<MultiSimulateViewer> {

	////////////////////////////////////////////
	//
	// Variable
	//
	////////////////////////////////////////////

	[SerializeField] SimulateViewer m_pfSimulateViewer;
	[SerializeField] int			m_InitialSimulateViewerCount;

	LinkedList<SimulateViewer>	m_UsingViewers;
	LinkedList<SimulateViewer>	m_TempViewers;


	////////////////////////////////////////////
	//
	// Function
	//
	////////////////////////////////////////////

	protected override void Awake() {
		base.Awake();
		m_UsingViewers = new LinkedList<SimulateViewer>();
		m_TempViewers = new LinkedList<SimulateViewer>();
		ExpandUnit(m_InitialSimulateViewerCount);
	}

	void ExpandUnit(int count) {
		for(int i = 0; i<count; ++i) {
			var unit = Instantiate(m_pfSimulateViewer, transform);
			m_TempViewers.AddLast(unit);
		}
	}

	public void Simulate(VoxelData voxelData, Action fEndDrop) {
		if(m_TempViewers.Count == 0) {
			ExpandUnit(1);
		}
		var unit = m_TempViewers.First.Value;
		m_TempViewers.RemoveFirst();
		m_UsingViewers.AddLast(unit);
		unit.Simulate(voxelData, this.OnEndSimulation, fEndDrop);
	}

	public void Stop() {
		foreach(var simulateViewer in m_UsingViewers) {
			simulateViewer.Stop();
			m_TempViewers.AddLast(simulateViewer);
		}
		m_UsingViewers.Clear();
	}

	void OnEndSimulation(SimulateViewer simulationViewer) {
		m_UsingViewers.Remove(simulationViewer);
		m_TempViewers.AddLast(simulationViewer);
	}

}
