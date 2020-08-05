using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class Plant {
	public int x;
	public int y;
	public Mesh mesh;
	public float growth;
	public bool reserved;
	public bool harvested;
	public Matrix4x4 matrix;
	public Quaternion rotation;
	public int index;
	public int seed;

	Random r;

	public static Dictionary<int,Mesh> meshLookup;

	public void Init(int posX, int posY, int randSeed) {
		r = new Random((uint)randSeed);
		seed = randSeed;
		if (meshLookup==null) {
			meshLookup = new Dictionary<int,Mesh>();
		}
		x = posX;
		mesh = GetMesh(randSeed);
		growth = 0f;
		y = posY;
		harvested = false;
		reserved = false;
		Vector3 worldPos = new Vector3(posX+.5f,0f,posY+.5f);
		rotation=Quaternion.Euler(r.NextFloat(-5, 5), r.NextFloat(0, 360),r.NextFloat(-5, 5));
		matrix = Matrix4x4.TRS(worldPos,rotation,Vector3.one);
	}

	public void EaseToWorldPosition(float x,float y,float z,float smooth) {
		matrix.m03 += (x - matrix.m03) * smooth*3f;
		matrix.m13 += (y - matrix.m13) * smooth*3f;
		matrix.m23 += (z - matrix.m23) * smooth*3f;
		ApplyMatrixToFarm();
	}
	public void ApplyMatrixToFarm() {
		// Farm.plantMatrices[seed][index / Farm.instancesPerBatch][index % Farm.instancesPerBatch] = matrix;
	}

	Mesh GetMesh(int seed) {
		Mesh output;
		if (meshLookup.TryGetValue(seed,out output)) {
			return output;
		} else {
			return GenerateMesh(seed);
		}
	}

	Mesh GenerateMesh(int seed) {
		List<Vector3> vertices = new List<Vector3>();
		List<int> triangles = new List<int>();
		List<UnityEngine.Color> colors = new List<UnityEngine.Color>();
		List<Vector2> uv = new List<Vector2>();

		UnityEngine.Color color1 = UnityEngine.Color.HSVToRGB(r.NextFloat(), r.NextFloat(.5f, .8f), r.NextFloat(.25f, .9f));
		UnityEngine.Color color2 = UnityEngine.Color.HSVToRGB(r.NextFloat(), r.NextFloat(.5f, .8f), r.NextFloat(.25f, .9f));

		float height = r.NextFloat(.4f,1.4f);

		float angle = r.NextFloat(0, 1) * Mathf.PI * 2f;
		float armLength1 = r.NextFloat(0, 1) * .4f + .1f;
		float armLength2 = r.NextFloat(0, 1) * .4f + .1f;
		float armRaise1 = r.NextFloat(0, 1) * .3f;
		float armRaise2 = r.NextFloat(0, 1) * .6f - .3f;
		float armWidth1 = r.NextFloat(0, 1) * .5f + .2f;
		float armWidth2 = r.NextFloat(0, 1) * .5f + .2f;
		float armJitter1 = r.NextFloat(0, 1) * .3f;
		float armJitter2 = r.NextFloat(0, 1) * .3f;
		float stalkWaveStr = r.NextFloat(0, 1) * .5f;
		float stalkWaveFreq = UnityEngine.Random.Range(.25f,1f);
		float stalkWaveOffset = r.NextFloat(0, 1) * Mathf.PI * 2f;

		int triCount = UnityEngine.Random.Range(15,35);

		for (int i=0;i<triCount;i++) {
			// front face
			triangles.Add(vertices.Count);
			triangles.Add(vertices.Count+1);
			triangles.Add(vertices.Count+2);

			// back face
			triangles.Add(vertices.Count + 1);
			triangles.Add(vertices.Count);
			triangles.Add(vertices.Count + 2);

			float t = i / (triCount-1f);
			float armLength = Mathf.Lerp(armLength1,armLength2,t);
			float armRaise = Mathf.Lerp(armRaise1,armRaise2,t);
			float armWidth = Mathf.Lerp(armWidth1,armWidth2,t);
			float armJitter = Mathf.Lerp(armJitter1,armJitter2,t);
			float stalkWave = Mathf.Sin(t*stalkWaveFreq*2f*Mathf.PI+stalkWaveOffset) * stalkWaveStr;

			float y = t * height;
			vertices.Add(new Vector3(stalkWave,y,0f));
			Vector3 armPos = new Vector3(stalkWave + Mathf.Cos(angle)*armLength,y + armRaise,Mathf.Sin(angle)*armLength);
			vertices.Add(armPos + UnityEngine.Random.insideUnitSphere * armJitter);
			armPos = new Vector3(stalkWave + Mathf.Cos(angle+armWidth) * armLength,y + armRaise,Mathf.Sin(angle+armWidth) * armLength);
			vertices.Add(armPos+UnityEngine.Random.insideUnitSphere*armJitter);

			colors.Add(color1);
			colors.Add(color2);
			colors.Add(color2);
			uv.Add(Vector2.zero);
			uv.Add(Vector2.right);
			uv.Add(Vector2.right);

			// golden angle in radians
			angle += 2.4f;
		}

		Mesh outputMesh = new Mesh();
		outputMesh.name = "Generated Plant (" + seed + ")";

		outputMesh.SetVertices(vertices);
		outputMesh.SetColors(colors);
		outputMesh.SetTriangles(triangles,0);
		outputMesh.RecalculateNormals();

		meshLookup.Add(seed,outputMesh);

		return outputMesh;
	}
}
