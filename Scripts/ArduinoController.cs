using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

// 아두이노 연결을 담당한다.
public class ArduinoController : MonoBehaviour {

	//////////////////////////////////
	//
	// Private Variable
	//
	//////////////////////////////////

	[SerializeField] string m_PortName;
	[SerializeField] int	m_BuadRate;

	SerialPort m_Port;


	//////////////////////////////////
	//
	// Property
	//
	//////////////////////////////////

	public SerialPort serialPort { get { return m_Port; } }


	//////////////////////////////////
	//
	// Function
	//
	//////////////////////////////////

	private void Awake() {
		m_Port = new SerialPort(m_PortName, m_BuadRate);
		try {
			m_Port.Open();
		}
		catch {
			print(string.Format("PortName : {0} does not exist!", m_PortName));
			m_Port = null;
		}
	}

	private void OnDestroy() {
		if(m_Port != null && m_Port.IsOpen) {
			m_Port.Close();
		}
	}

}
