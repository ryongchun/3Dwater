using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

[System.Serializable]
public struct QuadraticEquation {

	////////////////////////////////////////////
	//
	// Variable
	//
	////////////////////////////////////////////
	
	float a, b, c;


	////////////////////////////////////////////
	//
	// Function
	//
	////////////////////////////////////////////

	// Constructor
	public QuadraticEquation( float a, float b, float c ) {
		this.a = a;
		this.b = b;
		this.c = c;
	}

	// x값으로 y값을 구한다.
	public double GetValue( double x ) {
		return a * x * x + b * x + c;
	}

	// y값으로 x값을 구한다.
	public double GetInverseValue( double y ) {
		double value0 = -b / ( 2.0 * a );
		double value1 = (double)Math.Sqrt(b * b - ( 4.0 * a * ( c - y ) )) / ( 2.0 * a );
		return value0 + ( a > 0.0 ? value1 : -value1 );
	}

	public override string ToString() {
		var sb = new StringBuilder();
		sb.Append(string.Format("{0:0.00}x^2", a));
		if (b >= 0.0f)
			sb.Append("+");
		sb.Append(string.Format("{0:0.00}x", b));
		if (c >= 0.0f)
			sb.Append("+");
		sb.Append(string.Format("{0:0.00}", c));
		return sb.ToString();
	}
}
