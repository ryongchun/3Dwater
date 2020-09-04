using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformationMgr : MonoSingleton<InformationMgr> {

	////////////////////////////////////////////
	//
	// Variable
	//
	////////////////////////////////////////////

	[SerializeField] Vector3	m_SpaceSize;
	[SerializeField] int		m_WidthNozzleCount;
	[SerializeField] int		m_LengthNozzleCount;
	[SerializeField] bool		m_UseDropSound;
	[SerializeField] float		m_DropSoundDelay;

	QuadraticEquation	m_DropEquation;
	List<float>			m_SliceHeights;
	GameObject			m_Model;
	VoxelData			m_VoxelData;


	////////////////////////////////////////////
	//
	// Property
	//
	////////////////////////////////////////////

	public Vector3				spaceSize			{ get { return m_SpaceSize; } }
	public int					widthNozzleCount	{ get { return m_WidthNozzleCount; } }
	public int					lengthNozzleCount	{ get { return m_LengthNozzleCount; } }
	public QuadraticEquation	dropEquation		{ get { return m_DropEquation; } }
	public List<float>			sliceHeights		{ get { return m_SliceHeights; } }
	public bool					useDropSound		{ get { return m_UseDropSound; } }
	public float				dropSoundDelay		{ get { return m_DropSoundDelay; } }


	////////////////////////////////////////////
	//
	// Function
	//
	////////////////////////////////////////////

	InformationMgr() : base() {
		m_SpaceSize			= new Vector3(1.0f, 1.0f, 1.0f);
		m_WidthNozzleCount	= 16;
		m_LengthNozzleCount = 16;
		m_SliceHeights		= new List<float>();
		m_DropEquation		= new QuadraticEquation(4.32780245f, 0.37945823f, -0.02767264f);
		CalcuateSliceHeights();
	}

	void CalcuateSliceHeights() {
		m_SliceHeights.Clear();
		var time = m_DropEquation.GetInverseValue(m_SpaceSize.y);
		double interval = 0.021;
		while (time - interval > 0) {
			time -= interval;
			var height = m_DropEquation.GetValue(time);
			if (height < 0.0f)
				break;
			m_SliceHeights.Add((float)height);
		}
		m_SliceHeights.Add(0.0f);
	}

	void OnValidate() {
		if(UnityEditor.EditorApplication.isPlaying == true) {
			return;
		}
		// 슬라이스를 다시 계산한다.
		CalcuateSliceHeights();
		// 상자 크기를 바꾼다.
		var gridBox = GameObject.Find("GridBox");
		gridBox.transform.localScale = m_SpaceSize;
		// 카메라를 조정한다.
		var normalCamera = GameObject.Find("NormalCamera");
		CameraUtility.Refresh(normalCamera, m_SpaceSize);
		var simulateCamera = GameObject.Find("SimulateCamera");
		CameraUtility.Refresh(simulateCamera, m_SpaceSize);
		// 슬라이스 그리드의 높이를 조절한다.
		var sliceGridViewer = GameObject.Find("SliceGridViewer").GetComponent<SliceGridViewer>();
		sliceGridViewer.Refresh(m_SliceHeights, m_SpaceSize);
	}
}
