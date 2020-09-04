using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelizeUtility : MonoSingleton<VoxelizeUtility> {

	////////////////////////////////////////////
	//
	// Variable
	//
	////////////////////////////////////////////

	[SerializeField] Camera		m_Camera;
	[SerializeField] GameObject m_ModelObject;


	////////////////////////////////////////////
	//
	// Function
	//
	////////////////////////////////////////////

	public VoxelData ToVoxelData(InformationMgr informationMgr, GameObject model) {
		// 기본 정보를 가져온다.
		var sliceHeights	= informationMgr.sliceHeights;
		int widthCount		= informationMgr.widthNozzleCount;
		int lengthCount		= informationMgr.lengthNozzleCount;
		int heightCount		= sliceHeights.Count;
		var data			= new bool[widthCount * 2, heightCount, lengthCount];
		var spaceSize		= informationMgr.spaceSize;
		var interval		= new Vector3(spaceSize.x / widthCount / 2.0f, 0.0f, spaceSize.z / lengthCount);

		// 오브젝트 초기화
		m_ModelObject.SetActive(true);
		m_ModelObject.GetComponent<MeshFilter>().sharedMesh = model.GetComponent<MeshFilter>().sharedMesh;
		var modelTransform = m_ModelObject.transform;
		modelTransform.position = model.transform.position;
		modelTransform.eulerAngles = model.transform.eulerAngles;
		modelTransform.localScale = model.transform.localScale;

		// 렌더 텍스쳐와 정보를 가져올2d텍스쳘를 만든다.
		// h(아래->위), w(왼쪽->오른쪽), l(앞->뒤)
		m_Camera.gameObject.SetActive(true);
		var cameraTransform = m_Camera.transform;
		var hRT = new RenderTexture(widthCount * 2, lengthCount, 16) {
			wrapMode = TextureWrapMode.Clamp,
			filterMode = FilterMode.Point
		};
		var hTex2D = new Texture2D(widthCount * 2, lengthCount) {
			wrapMode = TextureWrapMode.Clamp,
			filterMode = FilterMode.Point
		};
		var wRT = new RenderTexture(lengthCount, 1, 16) {
			wrapMode = TextureWrapMode.Clamp,
			filterMode = FilterMode.Point
		};
		var wTex2D = new Texture2D(lengthCount, 1) {
			wrapMode = TextureWrapMode.Clamp,
			filterMode = FilterMode.Point
		};
		var lRT = new RenderTexture(widthCount * 2, 1, 16) {
			wrapMode = TextureWrapMode.Clamp,
			filterMode = FilterMode.Point
		};
		var lTex2D = new Texture2D(widthCount * 2, 1) {
			wrapMode = TextureWrapMode.Clamp,
			filterMode = FilterMode.Point
		};

		// 아래 -> 위
		cameraTransform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
		m_Camera.targetTexture = hRT;
		RenderTexture.active = hRT;
		m_Camera.orthographicSize = spaceSize.z / 2.0f;
		m_Camera.aspect = spaceSize.x / spaceSize.z;
		var source = new Rect(0.0f, 0.0f, widthCount * 2, lengthCount);
		var startPos = Vector3.zero;
		// 아래에서 위쪽으로 캡쳐한다. 
		for (int yInd = 0; yInd < heightCount; ++yInd) {
			cameraTransform.position = new Vector3(0.0f, spaceSize.y - sliceHeights[yInd], 0.0f);
			if (yInd == 0)
				m_Camera.farClipPlane = spaceSize.y - sliceHeights[yInd];
			else
				m_Camera.farClipPlane = sliceHeights[yInd-1] - sliceHeights[yInd];
			m_Camera.Render();
			hTex2D.ReadPixels(source, 0, 0);
			hTex2D.Apply();
			// 렌더텍스쳐에서 데이터를 가져온다.
			for (int zInd = 0; zInd < lengthCount; ++zInd) {
				for (int xInd = 0; xInd < widthCount * 2; ++xInd) {
					data[xInd, yInd, zInd] = ( hTex2D.GetPixel(xInd, zInd).a > 0.5f );
				}
			}
		}

		// 왼쪽 -> 오른쪽
		cameraTransform.eulerAngles = new Vector3(0.0f, -90.0f, 0.0f);
		m_Camera.farClipPlane = interval.x;
		m_Camera.targetTexture = wRT;
		RenderTexture.active = wRT;
		source = new Rect(0.0f, 0.0f, lengthCount, 1);
		startPos = new Vector3(-spaceSize.x / 2.0f, 0.0f, 0.0f);
		for (int xInd = 0; xInd < widthCount * 2; ++xInd) {
			for (int yInd = 0; yInd < heightCount; ++yInd) {
				float height = 0;
				if (yInd == 0)
					height = ( 0.0f + (spaceSize.y - sliceHeights[yInd]) ) / 2.0f;
				else
					height = ( (spaceSize.y - sliceHeights[yInd - 1]) + (spaceSize.y - sliceHeights[yInd]) ) / 2.0f;
				cameraTransform.position = startPos + new Vector3(interval.x * ( xInd + 1 ), height, 0.0f);
				if (yInd == 0)
					m_Camera.orthographicSize = (spaceSize.y - sliceHeights[yInd]) / 2.0f;
				else
					m_Camera.orthographicSize = ( sliceHeights[yInd-1] - sliceHeights[yInd] ) / 2.0f;
				m_Camera.aspect = ( spaceSize.z / 2.0f ) / ( m_Camera.orthographicSize );
				m_Camera.Render();
				wTex2D.ReadPixels(source, 0, 0);
				wTex2D.Apply();
				for (int zInd = 0; zInd < lengthCount; ++zInd) {
					data[xInd, yInd, zInd] |= ( wTex2D.GetPixel(zInd, 0).a > 0.5f );
				}
			}
		}

		// 앞 -> 뒤
		cameraTransform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
		m_Camera.farClipPlane = interval.z;
		m_Camera.targetTexture = lRT;
		RenderTexture.active = lRT;
		source = new Rect(0.0f, 0.0f, widthCount * 2, 1);
		startPos = new Vector3(0.0f, 0.0f, -spaceSize.z / 2.0f);
		for (int zInd = 0; zInd < lengthCount; ++zInd) {
			for (int yInd = 0; yInd < heightCount; ++yInd) {
				float height = 0;
				if (yInd == 0)
					height = ( spaceSize.y - sliceHeights[yInd] ) / 2.0f;
				else
					height = ( ( spaceSize.y - sliceHeights[yInd - 1] ) + ( spaceSize.y - sliceHeights[yInd] ) ) / 2.0f;
				cameraTransform.position = startPos + new Vector3(0.0f, height, interval.z * zInd);
				if (yInd == 0)
					m_Camera.orthographicSize = ( spaceSize.y - sliceHeights[yInd] ) / 2.0f;
				else
					m_Camera.orthographicSize = ( sliceHeights[yInd - 1] - sliceHeights[yInd] ) / 2.0f;
				m_Camera.aspect = ( spaceSize.x / 2.0f ) / ( m_Camera.orthographicSize );
				m_Camera.Render();
				lTex2D.ReadPixels(source, 0, 0);
				lTex2D.Apply();
				for (int xInd = 0; xInd < widthCount * 2; ++xInd) {
					data[xInd, yInd, zInd] |= ( lTex2D.GetPixel(xInd, 0).a > 0.5f );
				}
			}
		}

		m_ModelObject.SetActive(false);
		m_Camera.gameObject.SetActive(false);
		var voxelData = new VoxelData(widthCount, heightCount, lengthCount, data);
		return voxelData;
	}
}
