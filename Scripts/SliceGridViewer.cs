using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliceGridViewer : MonoBehaviour {

	////////////////////////////////////////////
	//
	// Variable
	//
	////////////////////////////////////////////

	[SerializeField] GameObject m_pfSliceObject;

	LinkedList<GameObject>	m_Slices;


	////////////////////////////////////////////
	//
	// Function
	//
	////////////////////////////////////////////

	SliceGridViewer() {
		m_Slices = new LinkedList<GameObject>();
	}

	public void Refresh(List<float> heights, Vector3 spaceSize) {
		// 현재 가지고 있는 자식객체를 삭제한다.
		var children = GetComponentsInChildren<Transform>();
		for (int i = 1; i < children.Length; ++i)
			StartCoroutine(Destroy(children[i].gameObject));
		m_Slices.Clear();
		// 슬라이스를 추가한다.
		for(int i = 0; i<heights.Count; ++i) {
			StartCoroutine(Create(new Vector3(0.0f, spaceSize.y - heights[i], 0.0f), new Vector3(spaceSize.x, 1.0f, spaceSize.z)));
		}
		StartCoroutine(SaveScene());
	}

	IEnumerator Destroy( GameObject go ) {
		yield return new WaitForEndOfFrame();
		DestroyImmediate(go);
	}

	IEnumerator Create( Vector3 pos, Vector3 scale) {
		yield return new WaitForEndOfFrame();
		var obj = Instantiate(m_pfSliceObject, transform);
		obj.transform.position = pos;
		obj.transform.localScale = scale;
		m_Slices.AddLast(obj);
	}

	IEnumerator SaveScene() {
		yield return new WaitForEndOfFrame();
		UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());

	}
}
