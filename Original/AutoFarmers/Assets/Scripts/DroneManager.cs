using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneManager : MonoBehaviour {
	public Mesh droneMesh;
	public Material droneMaterial;
	public int maxDroneCount;
	[Range(0f,1f)]
	public float moveSmooth;
	[Range(0f,1f)]
	public float carrySmooth;

	List<Drone> drones;
	List<List<Matrix4x4>> matrices;
	int batchNumber = 0;

	[SerializeField]
	public static int droneCount;
	public static DroneManager instance;

	public static void SpawnDrone(int x, int y) {
		Vector3 worldPos = new Vector3(x + .5f,0f,y + .5f);
		Drone drone = new Drone(worldPos);
		instance.matrices[instance.batchNumber].Add(drone.matrix);
		if (instance.matrices[instance.batchNumber].Count == Farm.instancesPerBatch) {
			instance.matrices.Add(new List<Matrix4x4>(Farm.instancesPerBatch));
			instance.batchNumber++;
		}
		instance.drones.Add(drone);
		droneCount = instance.drones.Count;
	}

	private void Awake() {
		instance = this;
	}
	void Start () {
		drones = new List<Drone>();
		matrices = new List<List<Matrix4x4>>();
		matrices.Add(new List<Matrix4x4>(Farm.instancesPerBatch));
	}
	
	void Update () {
		float deltaTime = Time.deltaTime;
		float moveSmoothing = 1f - Mathf.Pow(moveSmooth,Time.deltaTime);
		float carrySmoothing = 1f - Mathf.Pow(carrySmooth,Time.deltaTime);
		for (int i=0;i<drones.Count;i++) {
			Drone drone = drones[i];
			drone.Tick(moveSmoothing,carrySmoothing);
			matrices[i / Farm.instancesPerBatch][i % Farm.instancesPerBatch] = drone.matrix;
		}

		for (int i=0;i<matrices.Count;i++) {
			Graphics.DrawMeshInstanced(droneMesh,0,droneMaterial,matrices[i]);
		}
	}
}
