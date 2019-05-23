using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerManager : MonoBehaviour {
	public Mesh cubeMesh;
	public Material cubeMaterial;
	public float spawnSpacing;
	public float spawnDistanceFromPit;

	List<Runner> runners;
	List<Runner> pooledRunners;

	public static float runSpeed = 10f;

	List<Matrix4x4[]> matrices;
	List<Vector4[]> colors;
	MaterialPropertyBlock matProps;

	const int instancesPerBatch=1023;

	int activeBatch = 0;
	int activeBatchCount = 0;

	float spawnAngle = 0f;

	int matricesPerRunner;

	void SpawnARunner() {
		float spawnRadius = PitGenerator.pitRadius + spawnDistanceFromPit;
		Vector3 pos = new Vector3(Mathf.Cos(spawnAngle) * spawnRadius,0f,Mathf.Sin(spawnAngle) * spawnRadius);

		Runner runner;
		if (pooledRunners.Count == 0) {
			runner = new Runner();
		} else {
			runner = pooledRunners[pooledRunners.Count - 1];
			pooledRunners.RemoveAt(pooledRunners.Count - 1);
		}

		runner.Init(pos);
		runners.Add(runner);
		for (int i = 0; i < matricesPerRunner; i++) {
			if (activeBatchCount==instancesPerBatch) {
				activeBatch++;
				activeBatchCount = 0;
				if (matrices.Count == activeBatch) {
					matrices.Add(new Matrix4x4[instancesPerBatch]);
					colors.Add(new Vector4[instancesPerBatch]);
				}
			}
			activeBatchCount++;
		}

		int mode = Mathf.FloorToInt(Time.time / 8f);
		if (mode % 2 == 0) {
			spawnAngle = Random.value * 2f * Mathf.PI;
			spawnSpacing = Random.Range(.04f,Mathf.PI*.4f);
		} else {
			spawnAngle += spawnSpacing;
		}
	}

	void Start () {
		Runner tempRunner = new Runner();
		tempRunner.Init(Vector3.zero);
		matricesPerRunner = Runner.bars.Length / 2;
		runners = new List<Runner>();
		pooledRunners = new List<Runner>();
		matrices = new List<Matrix4x4[]>();
		colors = new List<Vector4[]>();
		matrices.Add(new Matrix4x4[instancesPerBatch]);
		colors.Add(new Vector4[instancesPerBatch]);
		matProps = new MaterialPropertyBlock();
		matProps.SetVectorArray("_Color",new Vector4[instancesPerBatch]);
	}

	private void FixedUpdate() {
		for (int i = 0; i < 2; i++) {
			SpawnARunner();
		}

		float runDirSway = Mathf.Sin(Time.time * .5f) * .5f;
		for (int i=0;i<runners.Count;i++) {
			runners[i].Update(runDirSway);
			if (runners[i].dead) {
				for (int j=0;j<matricesPerRunner;j++) {
					activeBatchCount--;
					if (activeBatchCount==0 && activeBatch>0) {
						activeBatch--;
						activeBatchCount = 1023;
					}
				}
				pooledRunners.Add(runners[i]);
				runners.RemoveAt(i);
				i--;
			}
		}
	}

	void Update () {
		for (int i = 0; i < runners.Count; i++) {
			Runner runner = runners[i];
			for (int j = 0; j < Runner.bars.Length/2; j++) {
				Vector3 point1 = runner.points[Runner.bars[j*2]];
				Vector3 point2 = runner.points[Runner.bars[j*2 + 1]];
				Vector3 oldPoint1 = runner.prevPoints[Runner.bars[j * 2]];
				Vector3 oldPoint2 = runner.prevPoints[Runner.bars[j*2 + 1]];

				float t = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;
				point1 += (point1 - oldPoint1) * t;
				point2 += (point2 - oldPoint2) * t;

				Vector3 delta = point2 - point1;
				Vector3 position = (point1 + point2) * .5f;
				Quaternion rotation = Quaternion.LookRotation(delta);
				Vector3 scale = new Vector3(Runner.barThicknesses[j]*runner.timeSinceSpawn,
											Runner.barThicknesses[j]*runner.timeSinceSpawn,
											Mathf.Sqrt(delta.x*delta.x+delta.y*delta.y+delta.z*delta.z)*runner.timeSinceSpawn);
				int index = i * matricesPerRunner + j;
				matrices[index/instancesPerBatch][index%instancesPerBatch] = Matrix4x4.TRS(position,rotation,scale);
				colors[index / instancesPerBatch][index % instancesPerBatch] = runner.color;
			}
		}

		for (int i = 0; i <= activeBatch; i++) {
			int batchSize = instancesPerBatch;
			if (i==activeBatch) {
				batchSize = activeBatchCount;
			}
			matProps.SetVectorArray("_Color",colors[i]);
			Graphics.DrawMeshInstanced(cubeMesh,0,cubeMaterial,matrices[i],batchSize,matProps);
		}
	}
}
