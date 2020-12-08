using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpawnerAuthoring : MonoBehaviour
{
	public Mesh particleMesh;
	public Material particleMaterial;
	public float spinRate;
	public float upwardSpeed;
	Vector3[] points;
	Matrix4x4[] matrices;
	MaterialPropertyBlock matProps;
	float[] radiusMults;

	private void Start()
	{
		points = new Vector3[1000];
		matrices = new Matrix4x4[1000];
		radiusMults = new float[1000];
		Vector4[] colors = new Vector4[1000];

		for (int i = 0; i < points.Length; i++)
		{
			Vector3 pos = new Vector3(Random.Range(-50f, 50f), Random.Range(0f, 50f), Random.Range(-50f, 50f));
			points[i] = pos;
			matrices[i] = Matrix4x4.TRS(points[i], Quaternion.identity, Vector3.one * Random.Range(.2f, .7f));
			radiusMults[i] = Random.value;
			colors[i] = Color.white * Random.Range(.3f, .7f);
		}
		matProps = new MaterialPropertyBlock();
		matProps.SetVectorArray("_Color", colors);
	}
}
