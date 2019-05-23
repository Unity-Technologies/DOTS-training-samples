using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bar {
	public Point point1;
	public Point point2;
	public float length;
	public Matrix4x4 matrix;
	public float oldDX;
	public float oldDY;
	public float oldDZ;
	public float minX;
	public float maxX;
	public float minY;
	public float maxY;
	public float minZ;
	public float maxZ;
	public Color color;
	public float thickness;

	public void AssignPoints(Point a, Point b) {
		point1 = a;
		point2 = b;
		Vector3 delta = new Vector3(point2.x - point1.x,point2.y - point1.y,point2.z - point1.z);
		length = delta.magnitude;

		thickness = Random.Range(.25f,.35f);

		Vector3 pos = new Vector3(point1.x + point2.x,point1.y + point2.y,point1.z + point2.z) * .5f;
		Quaternion rot = Quaternion.LookRotation(delta);
		Vector3 scale = new Vector3(thickness,thickness,length);
		matrix = Matrix4x4.TRS(pos,rot,scale);

		minX = Mathf.Min(point1.x,point2.x);
		maxX = Mathf.Max(point1.x,point2.x);
		minY = Mathf.Min(point1.y,point2.y);
		maxY = Mathf.Max(point1.y,point2.y);
		minZ = Mathf.Min(point1.z,point2.z);
		maxZ = Mathf.Max(point1.z,point2.z);

		float upDot = Mathf.Acos(Mathf.Abs(Vector3.Dot(Vector3.up,delta.normalized)))/Mathf.PI;
		color = Color.white * upDot*Random.Range(.7f,1f);
	}
}
