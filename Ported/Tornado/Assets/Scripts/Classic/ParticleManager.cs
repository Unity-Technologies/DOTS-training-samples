using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParticleManager : MonoBehaviour {
	public Mesh particleMesh;
	public Material particleMaterial;
	public float spinRate;
	public float upwardSpeed;
	Vector3[] points;
	Matrix4x4[] matrices;
	MaterialPropertyBlock matProps;
	float[] radiusMults;

	public int particlesCount = 1000;
	Vector4[] colors;

	private void Start() {
		points = new Vector3[particlesCount];
		matrices = new Matrix4x4[particlesCount];
		radiusMults = new float[particlesCount];
		colors = new Vector4[particlesCount];

		for (int i=0;i<points.Length;i++) {
			Vector3 pos = new Vector3(Random.Range(-50f,50f),Random.Range(0f,50f),Random.Range(-50f,50f));
			points[i] = pos;
			matrices[i] = Matrix4x4.TRS(points[i],Quaternion.identity,Vector3.one*Random.Range(.2f,.7f));
			radiusMults[i] = Random.value;
			colors[i] = Color.white * Random.Range(.3f,.7f);
		}
		matProps = new MaterialPropertyBlock();
		matProps.SetVectorArray("_BaseColor", colors);
	}

	void Update() {
		for (int i=0;i<points.Length;i++) {
			Vector3 tornadoPos = new Vector3(PointManager.tornadoX+PointManager.TornadoSway(points[i].y),points[i].y,PointManager.tornadoZ);
			Vector3 delta = (tornadoPos - points[i]);
			float dist = delta.magnitude;
			delta /= dist;
			float inForce = dist - Mathf.Clamp01(tornadoPos.y / 50f)*30f*radiusMults[i]+2f;
			points[i] += new Vector3(-delta.z*spinRate+delta.x*inForce,upwardSpeed,delta.x*spinRate+delta.z*inForce)*Time.deltaTime;
			if (points[i].y>50f) {
				points[i] = new Vector3(points[i].x,0f,points[i].z);
			}

			Matrix4x4 matrix = matrices[i];
			matrix.m03 = points[i].x;
			matrix.m13 = points[i].y;
			matrix.m23 = points[i].z;
			matrices[i] = matrix;
		}

		var chunkCount = 1023;

		for (int i = 0; i < matrices.Length; i += chunkCount)
		{
			var tmpMatrices = matrices.Where((m, idx) =>
			{
				return idx >= i && idx < (i + chunkCount);
			}).ToArray();

			var tmpMatProps = new MaterialPropertyBlock();
			var tmpColors = colors.Where((c, idx) => {
				return idx >= i && idx < (i + chunkCount);
			}).ToArray();
			tmpMatProps.SetVectorArray("_BaseColor", tmpColors);

			Graphics.DrawMeshInstanced(particleMesh, 0, particleMaterial, tmpMatrices, tmpMatrices.Length);
		}
	}
}
