using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelData {

	////////////////////////////////////////////
	//
	// Variable
	//
	////////////////////////////////////////////

	int m_Height, m_Width, m_Length;
	bool[,,] m_Data;


	////////////////////////////////////////////
	//
	// Property
	//
	////////////////////////////////////////////

	public int	height	{ get { return m_Height; } }
	public int	width	{ get { return m_Width; } }
	public int	length	{ get { return m_Length; } }
	public bool this[int wInd, int hInd, int lInd] { get { return m_Data[wInd, hInd, lInd]; } }


	////////////////////////////////////////////
	//
	// Function
	//
	////////////////////////////////////////////

	public VoxelData(int width, int height, int length, bool[,,] data) {
		m_Width		= width;
		m_Height	= height;
		m_Length	= length;
		m_Data		= new bool[m_Width, m_Height, m_Length];
		for(int lInd = 0; lInd < m_Length; ++lInd) {
			for(int hInd = 0; hInd < m_Height; ++hInd) {
				for(int wInd = 0; wInd < m_Width * 2; ++wInd) {
					// 개수가 많은 줄이면(처음부터 시작)
					if(lInd % 2 == 0) {
						m_Data[wInd / 2, hInd, lInd] |= data[wInd, hInd, lInd];
					}
					// 개수가 적은 줄이면(2번째 칸부터 시작)
					else {
						if(wInd != 0 || wInd != m_Width * 2 - 1) {
							m_Data[( wInd - 1 ) / 2, hInd, lInd] |= data[wInd, hInd, lInd];
						}
					}
				}
			}
		}
	}
}
