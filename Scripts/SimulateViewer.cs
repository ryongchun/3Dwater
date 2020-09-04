using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class SimulateViewer : MonoBehaviour {

	////////////////////////////////////////////
	//
	// Variable
	//
	////////////////////////////////////////////

	[SerializeField] GameObject m_pfWaterDrop;
    [SerializeField] int        m_RepeatCount;
	[SerializeField] float		m_RepeatDelay;

	List<GameObject>	m_Layers;
	GameObject[,,]		m_WaterDrops;
	InformationMgr		m_InformationMgr;
	ArduinoController	m_ArduinoController;
	Coroutine			m_SimulateRoutine;
	Thread				m_SimulateThread;
	AudioSource			m_DropSound;


	////////////////////////////////////////////
	//
	// Function
	//
	////////////////////////////////////////////

	void Awake() {
		m_Layers			= new List<GameObject>();
		m_DropSound			= GetComponent<AudioSource>();
		m_InformationMgr	= InformationMgr.instance;
		m_ArduinoController = GameObject.Find("MainArduinoController").GetComponent<ArduinoController>();
		Init();
	}

	public void Init() {
		// 정보를 가져온다.
		int width			= m_InformationMgr.widthNozzleCount;
		int length			= m_InformationMgr.lengthNozzleCount;
		var sliceHeights	= m_InformationMgr.sliceHeights;
		int height			= sliceHeights.Count;
		m_WaterDrops		= new GameObject[width, height, length];
		var spaceSize		= m_InformationMgr.spaceSize;
		var interval		= new Vector2(spaceSize.x / width, spaceSize.z / length);
		// 물방울을 가장 작은 칸 크기로 한다.
		float waterDropSize = sliceHeights[sliceHeights.Count - 1] - sliceHeights[sliceHeights.Count - 2];
		m_pfWaterDrop.transform.localScale = Vector3.one * waterDropSize;
		// 큐브를 배치한다.
		for (int hInd = 0; hInd < height; ++hInd) {
			var go = new GameObject("Layer" + hInd);
			go.transform.parent = transform;
			m_Layers.Add(go);
			var pos = new Vector3();
			if (hInd == 0)
				pos.y = 0.0f;
			else
				pos.y = spaceSize.y - sliceHeights[hInd - 1];
			go.transform.position = pos;
            pos.y = 0.0f;
			for (int lInd = 0; lInd < length; ++lInd) {
				for (int wInd = 0; wInd < width; ++wInd) {
					if (lInd % 2 == 0) {
						pos.x = -spaceSize.x / 2.0f + interval.x / 2.0f + interval.x * wInd;
					}
					else {
						pos.x = -spaceSize.x / 2.0f + interval.x + interval.x * wInd;
					}
					pos.z = -spaceSize.z / 2.0f + interval.y / 2.0f + interval.y * lInd;
					var waterDrop = Instantiate(m_pfWaterDrop, go.transform);
					waterDrop.transform.localPosition = pos;
					m_WaterDrops[wInd, hInd, lInd] = waterDrop;
				}
			}
		}
	}

	public void Simulate(VoxelData voxelData, Action<SimulateViewer> fEndSimulation=null, Action fEndDrop = null) {
		if (m_SimulateRoutine != null)
			return;

		for (int lInd = 0; lInd < m_WaterDrops.GetLength(2); ++lInd) {
			for (int hInd = 0; hInd < m_WaterDrops.GetLength(1); ++hInd) {
				for (int wInd = 0; wInd < m_WaterDrops.GetLength(0); ++wInd) {
					m_WaterDrops[wInd, hInd, lInd].SetActive(voxelData[wInd, hInd, lInd]);
				}
			}
		}

		gameObject.SetActive(true);
		m_SimulateRoutine = StartCoroutine(UpdateSimulate(m_RepeatCount, voxelData, fEndSimulation, fEndDrop));
	}

	public void Stop() {
		if(m_SimulateRoutine != null) {
			StopCoroutine(m_SimulateRoutine);
			m_SimulateRoutine = null;
			gameObject.SetActive(false);
			if (m_SimulateThread != null) {
				m_SimulateThread.Abort();
				m_SimulateThread.Join();
			}
		}
	}

	IEnumerator UpdateSimulate(int repeatCount, VoxelData voxelData, Action<SimulateViewer> fEndSimulation, Action fEndDrop) {
		// 모든 오브젝트를 끄고, 위에 배치한다.
		var heightCount = m_InformationMgr.sliceHeights.Count; //슬라이스의 개수
		var height		= m_InformationMgr.spaceSize.y; //가상 공간의 높FullOpenAchieveTime
		for(int i = 0; i<m_Layers.Count; ++i) {
			m_Layers[i].transform.position = new Vector3(0.0f, height, 0.0f);
			m_Layers[i].SetActive(false);
		}
		// 아두이노로 신호를 보낸다.
		var startTime = DateTime.Now;
		Action updateArduino = () => {
			int width			= m_InformationMgr.widthNozzleCount; //하드웨어 가로 밸브개수 
			int length			= m_InformationMgr.lengthNozzleCount;// 하드웨어 세로 밸브개수
			var nozzleCount		= width * length - (length - 1) / 2 + 1; //밸브 순서(지그재그) 
			byte[] buffer		= new byte[( nozzleCount - 1 ) / 8 + 1]; //버퍼 길이
			var port			= m_ArduinoController.serialPort; //연결된 아두이노 포트주소
			float FullOpenAchieveTime = 6f; // 오픈신호를 인가하는 시점부터 밸브가 완전히 열리는 시간
			float FullOpenMaintainTime = 7f; // 밸브 close 명령내린후 완전히 close 될때까지의 시간 
			float FullCloseAchieveTime = 8f;//인접한 물방울 오픈신호와 다음 물방울 오픈신호 인가 시점 사이의 시간

			for (int hInd = 0; hInd < heightCount; ++hInd) {

                    // 시간이 됬는지 확인한다.(오픈신호 인가 시간체크)
                    while (true) {

					    // 제어프로그램에서 신호전송버튼 누르는 시간을 기준으로 0.021초 * 복섹 슬라이스의 순서 마다 시간을 체크하여 0.021초 간격으로 오픈신호를 보낸다 
						if (( DateTime.Now - startTime ).TotalMilliseconds < (FullOpenAchieveTime+ FullOpenMaintainTime + FullCloseAchieveTime) * hInd)
							
							continue;
						else
							break;
					}

					// 오픈신호를 보내는 시간이 되면 오픈신호 버퍼를 채운다.
					int orderInd = 0;
					for (int lInd = 0; lInd < length; ++lInd) {

					    //솔레노이드 뱁브의 배열은  지그재그로 첫줄은 21개 두번째줄은 20개이다.
						if (lInd % 2 == 0) {
							for (int wInd = 0; wInd < width; ++wInd) {
								if (voxelData[wInd, hInd, lInd])
									buffer[orderInd / 8] |= (byte)( 1 << ( orderInd % 8 ) );
								++orderInd;
							}
						}
						// 두번째 줄은 밸브 하나가 없기 때문에  해당하는 버퍼를 채울때 하나을 뺀다
						else {
							for(int wInd = width-2; wInd >= 0; --wInd) {
								if (voxelData[wInd, hInd, lInd])
									buffer[orderInd / 8] |= (byte)( 1 << ( orderInd % 8 ) );
								++orderInd;
							}
						}
					}
					port.Write(buffer, 0, buffer.Length);
					// 시간이 됬는지 확인한다.
					while (true) {

					    // 제어프로그램에서 신호전송버튼 누르는 시간을 기준으로 오픈신호를 전송하고 0.013초 후 클로즈 신호를 보낸다 
					    if (( DateTime.Now - startTime ).TotalMilliseconds < (FullOpenAchieveTime + FullOpenMaintainTime + FullCloseAchieveTime) * hInd + FullOpenAchieveTime + FullOpenMaintainTime)
                       
							continue;
						else
							break;
					}
					// 버퍼를 비운다.
					Array.Clear(buffer, 0, buffer.Length);
					port.Write(buffer, 0, buffer.Length);
			}
		};
		m_SimulateThread = null;
		if (m_ArduinoController.serialPort != null) {
			m_SimulateThread = new Thread(new ThreadStart(updateArduino));
			m_SimulateThread.Start();
		}
		// 사운드를 플레이 한다.
		StartCoroutine(PlayDropSound());

		// 시뮬레이션 한다.
		var dropEquation = m_InformationMgr.dropEquation;
		var sliceHeights = m_InformationMgr.sliceHeights;
		float time = 0.0f;
		float interval = 0.021f;
		int showIndex = 0;
		bool endDrop = false;
		while(showIndex != m_Layers.Count) {
			for(int i = showIndex; i<m_Layers.Count; ++i) {
				var localTime = time - interval * i;
				if (localTime < 0.0f)
					break;
				if(endDrop == false && i == m_Layers.Count-1) {
					endDrop = true;
					if(fEndDrop != null) {
						fEndDrop();
					}
				}
				var h = height - dropEquation.GetValue(localTime);
				var pos = new Vector3(0.0f, (float)h, 0.0f);
				m_Layers[i].SetActive(true);
				m_Layers[i].transform.position = pos;
				if(pos.y < 0.0f) {
					m_Layers[i].gameObject.SetActive(false);
					++showIndex;
				}
			}
			time += Time.deltaTime;
			yield return null;
		}
		if(m_SimulateThread != null) {
			while(true) {
				if(m_SimulateThread.Join(0)) {
					break;
				}
				yield return null;
			}
			m_SimulateThread = null;
		}
		if (m_RepeatDelay != 0.0f) {
			yield return new WaitForSeconds(m_RepeatDelay);
		}
        if(repeatCount != 1) {
            m_SimulateRoutine = StartCoroutine(UpdateSimulate(repeatCount-1, voxelData, fEndSimulation, fEndDrop));
        }
		else {
			gameObject.SetActive(false);
			m_SimulateRoutine = null;
			if(fEndSimulation != null) {
				fEndSimulation(this);
			}
		}
	}

	IEnumerator PlayDropSound() {
		if(m_InformationMgr.useDropSound == false) {
			yield break;
		}
		yield return new WaitForSeconds(m_InformationMgr.dropSoundDelay);
		m_DropSound.Play();
	}
}
