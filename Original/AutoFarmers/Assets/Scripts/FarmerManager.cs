using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmerManager : MonoBehaviour {
	public Mesh farmerMesh;
	public Material farmerMaterial;
	public int initialFarmerCount;
	public int maxFarmerCount;
	[Range(0f,1f)]
	public float movementSmooth;
	public Farmer firstFarmer;

	List<Farmer> farmers;
	List<Matrix4x4> farmerMatrices;
	
	public static FarmerManager instance;
	[SerializeField]
	public static int farmerCount;

	public static void SpawnFarmer() {
		Vector2Int mapSize = Farm.instance.mapSize;
		for (int i = 0; i < 100; i++) {
			Vector2Int spawnPos = new Vector2Int(Random.Range(0,mapSize.x),Random.Range(0,mapSize.y));
			if (Farm.IsBlocked(spawnPos) == false) {
				SpawnFarmer(spawnPos.x,spawnPos.y);
				break;
			}
		}
	}

	public static void SpawnFarmer(int x, int y) {
		Vector2 pos = new Vector2(x + .5f,y + .5f);
		Farmer farmer = new Farmer(pos);
		instance.farmers.Add(farmer);
		instance.farmerMatrices.Add(farmer.matrix);
		farmerCount++;
	}

	private void Awake() {
		instance = this;
	}
	void Start () {
		farmers = new List<Farmer>();
		farmerMatrices = new List<Matrix4x4>();
		farmerCount = 0;

		while (farmers.Count<initialFarmerCount) {
			SpawnFarmer();
		}
		firstFarmer = farmers[0];
	}
	
	void Update () {
		if (farmers.Count > 0) {
			float smooth = 1f - Mathf.Pow(movementSmooth,Time.deltaTime);
			for (int i = 0; i < farmers.Count; i++) {
				farmers[i].Tick(smooth);
				farmerMatrices[i] = farmers[i].matrix;
			}

			Graphics.DrawMeshInstanced(farmerMesh,0,farmerMaterial,farmerMatrices);
		}
	}
}
