using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T> {

	/////////////////////////////////////
	//
	// Private Variable
	//
	/////////////////////////////////////

	static T m_Instance;


	/////////////////////////////////////
	//
	// Property
	//
	/////////////////////////////////////

	public static T instance {
		get {
			if(m_Instance == null) {
				var obj = new GameObject(typeof(T).FullName);
				m_Instance = obj.AddComponent<T>();
			}
			return m_Instance;
		}
	}


	/////////////////////////////////////
	//
	// Function
	//
	/////////////////////////////////////

	protected MonoSingleton() {
		if (m_Instance == null) {
			m_Instance = this as T;
		}
	}

	protected virtual void Awake() {
		if(m_Instance == null) {
			m_Instance = this as T;
		}
		else if(m_Instance != this) {
			Destroy(gameObject);
		}
	}

	protected virtual void OnDestroy() {
		if (m_Instance == this) {
			m_Instance = null;
		}
	}

}