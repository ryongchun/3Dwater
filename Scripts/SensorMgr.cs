using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SensorMgr : MonoBehaviour {

	//////////////////////////////////
	//
	// Private Variable
	//
	//////////////////////////////////

	[SerializeField] UnityEvent[] m_Events;
	ArduinoController m_ArduinoController;


	//////////////////////////////////
	//
	// Function
	//
	//////////////////////////////////

	private void Awake() {
		m_ArduinoController = GameObject.Find("SensorArduinoController").GetComponent<ArduinoController>();
	}

	private void Start() {
		var port = m_ArduinoController.serialPort;
		port.ReadTimeout = 1;
	}

	private void Update() {
		var buffer = new byte[10];
		try {
			int readByte = m_ArduinoController.serialPort.Read(buffer, 0, buffer.Length);
			if (readByte != 0) {
				m_Events[buffer[0]].Invoke();
			}
		}
		catch { }
	}


}
