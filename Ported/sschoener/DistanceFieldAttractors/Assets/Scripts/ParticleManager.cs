using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour {
	public float attraction;
	public float speedStretch;
	public float jitter;
	public Mesh particleMesh;
	public Material particleMaterial;
	public Color surfaceColor;
	public Color interiorColor;
	public Color exteriorColor;
	public float exteriorColorDist = 3f;
	public float interiorColorDist = 3f;
	public float colorStiffness;
	Orbiter[] orbiters;

	Matrix4x4[][] matrices;
	Vector4[][] colors;
	MaterialPropertyBlock[] matProps;
	int finalBatchCount;

	const int instancesPerBatch = 1023;

	void Start () {
		finalBatchCount = 0;
		orbiters = new Orbiter[4000];
		matrices = new Matrix4x4[orbiters.Length/instancesPerBatch+1][];
		matrices[0] = new Matrix4x4[instancesPerBatch];
		colors = new Vector4[matrices.Length][];
		colors[0] = new Vector4[instancesPerBatch];

		int batch = 0;
		for (int i=0;i<orbiters.Length;i++) {
			orbiters[i]=new Orbiter(Random.insideUnitSphere*50f);
			finalBatchCount++;
			if (finalBatchCount==instancesPerBatch) {
				batch++;
				finalBatchCount = 0;
				matrices[batch]=new Matrix4x4[instancesPerBatch];
				colors[batch] = new Vector4[instancesPerBatch];
			}
		}
		matProps = new MaterialPropertyBlock[colors.Length];
		for (int i = 0; i <= batch; i++) {
			matProps[i] = new MaterialPropertyBlock();
			matProps[i].SetVectorArray("_Color",new Vector4[instancesPerBatch]);
		}
	}

	void FixedUpdate () {
		for (int i=0;i<orbiters.Length;i++) {
			Orbiter orbiter = orbiters[i];
			Vector3 normal;
			float dist = DistanceField.GetDistance(orbiter.position.x,orbiter.position.y,orbiter.position.z,out normal);
			orbiter.velocity -= normal.normalized * attraction * Mathf.Clamp(dist,-1f,1f);
			orbiter.velocity += Random.insideUnitSphere*jitter;
			orbiter.velocity *= .99f;
			orbiter.position += orbiter.velocity;
			Color targetColor;
			if (dist>0f) {
				targetColor = Color.Lerp(surfaceColor,exteriorColor,dist/exteriorColorDist);
			} else {
				targetColor = Color.Lerp(surfaceColor,interiorColor,-dist / interiorColorDist);
			}
			orbiter.color = Color.Lerp(orbiter.color,targetColor,Time.deltaTime * colorStiffness);
			orbiters[i] = orbiter;
		}
	}

	private void Update() {
		for (int i=0;i<orbiters.Length;i++) {
			Orbiter orbiter = orbiters[i];
			Vector3 scale = new Vector3(.1f,.01f,Mathf.Max(.1f,orbiter.velocity.magnitude * speedStretch));
			Matrix4x4 matrix = Matrix4x4.TRS(orbiter.position,Quaternion.LookRotation(orbiter.velocity),scale);
			matrices[i / instancesPerBatch][i % instancesPerBatch] = matrix;
			colors[i / instancesPerBatch][i % instancesPerBatch] = orbiter.color;
		}

		for (int i=0;i<matrices.Length;i++) {
			int count = instancesPerBatch;
			if (i==matrices.Length-1) {
				count = finalBatchCount;
			}
			matProps[i].SetVectorArray("_Color",colors[i]);
			Graphics.DrawMeshInstanced(particleMesh,0,particleMaterial,matrices[i],count,matProps[i]);
		}
	}
}
