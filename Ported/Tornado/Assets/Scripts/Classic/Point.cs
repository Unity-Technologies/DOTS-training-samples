using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point {
	public float x;
	public float y;
	public float z;

	public float oldX;
	public float oldY;
	public float oldZ;

	public bool anchor;

	public int neighborCount;

	public void CopyFrom(Point other) {
		x = other.x;
		y = other.y;
		z = other.z;
		oldX = other.oldX;
		oldY = other.oldY;
		oldZ = other.oldZ;

		anchor = other.anchor;
		neighborCount = other.neighborCount;
	}
}
