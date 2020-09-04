using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraUtility {

	////////////////////////////////////////////
	//
	// Function
	//
	////////////////////////////////////////////

	public static void Refresh(GameObject obj, Vector3 spaceSize) {
		var cam = obj.GetComponent<Camera>();
		// 박스 사이즈를 가져온다.
		var gridBoxSize = spaceSize;
		var size2d = new Vector2();
		size2d.x = Mathf.Sqrt(gridBoxSize.x * gridBoxSize.x + gridBoxSize.z * gridBoxSize.z);
		size2d.y = Mathf.Sqrt(gridBoxSize.x * gridBoxSize.x + gridBoxSize.z * gridBoxSize.z + gridBoxSize.y * gridBoxSize.y);
		// 크기를 구한다.
		var spaceFillRate = 0.7f;
		var screenSize = new Vector2(640.0f, 720.0f);
		float xRate = screenSize.x / size2d.x;
		float yRate = screenSize.y / size2d.y;
		if(xRate < yRate) {
			cam.orthographicSize = size2d.y * ( size2d.x / size2d.y ) / spaceFillRate * 0.5f;
		}
		else {
			cam.orthographicSize = size2d.y / spaceFillRate * 0.5f;
		}
		// 위치를 이동한다.
		var pos = new Vector3(size2d.y, size2d.y * 2.0f - gridBoxSize.y / 2.0f, -size2d.y);
		obj.transform.position = pos;
	}
}
