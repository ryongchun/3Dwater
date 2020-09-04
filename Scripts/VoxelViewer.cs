using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelViewer : MonoSingleton<VoxelViewer> {

	////////////////////////////////////////////
	//
	// Variable
	//
	////////////////////////////////////////////

	[SerializeField] GameObject m_pfCube;

	GameObject[,,]	m_Cubes;
	InformationMgr	m_InformationMgr;


	////////////////////////////////////////////
	//
	// Function
	//
	////////////////////////////////////////////

	protected override void Awake() {
		base.Awake();
		m_InformationMgr = InformationMgr.instance;
		Init();
	}

	public void Init() {
		// 정보를 가져온다.
		int width			= m_InformationMgr.widthNozzleCount;
		int length			= m_InformationMgr.lengthNozzleCount;
		var sliceHeights	= m_InformationMgr.sliceHeights;
		int height			= sliceHeights.Count;
		m_Cubes				= new GameObject[width, height, length];
		var spaceSize		= m_InformationMgr.spaceSize;
		var interval		= new Vector3(spaceSize.x / width, 0.0f,  spaceSize.z / length);
		// 큐브를 배치한다.
		var scale = new Vector3();
		scale.x = interval.x;
		scale.z = interval.z;
		for (int hInd = 0; hInd < height; ++hInd) {
			var pos = new Vector3();
			if (hInd == 0) {
				pos.y = ( 0.0f + ( spaceSize.y - sliceHeights[hInd] ) ) / 2.0f;
				scale.y = ( spaceSize.y - sliceHeights[hInd] );
			}
			else {
				scale.y = ( sliceHeights[hInd - 1] - sliceHeights[hInd] );
				pos.y = ( ( spaceSize.y - sliceHeights[hInd - 1] ) + ( spaceSize.y - sliceHeights[hInd] ) ) / 2.0f;
			}

			for (int lInd = 0; lInd < length; ++lInd) {
				for (int wInd = 0; wInd < width; ++wInd) {
					// 짝수줄인지 홀수줄인지 확인
					if (lInd % 2 == 0) {
						pos.x = -spaceSize.x / 2.0f + interval.x / 2.0f + interval.x * wInd;
					}
					else {
						pos.x = -spaceSize.x / 2.0f + interval.x + interval.x * wInd;
					}
					//
					pos.z = -spaceSize.z / 2.0f + interval.z / 2.0f + interval.z * lInd;
					var cube = Instantiate(m_pfCube, transform);
					cube.transform.position = pos;
					cube.transform.localScale = scale;
					m_Cubes[wInd, hInd, lInd] = cube;
				}
			}
		}
	}

	public void Show(VoxelData voxelData) {
		gameObject.SetActive(true);
		for (int lInd = 0; lInd < m_Cubes.GetLength(2); ++lInd) {
			for (int hInd = 0; hInd < m_Cubes.GetLength(1); ++hInd) {
				for (int wInd = 0; wInd < m_Cubes.GetLength(0); ++wInd) {
					m_Cubes[wInd, hInd, lInd].SetActive(voxelData[wInd, hInd, lInd]);
				}
			}
		}
	}

	public void Hide() {
		gameObject.SetActive(false);
	}

}
