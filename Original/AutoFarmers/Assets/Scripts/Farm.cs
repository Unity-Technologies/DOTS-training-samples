using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farm : MonoBehaviour {
	public Vector2Int mapSize;
	public int storeCount;
	public int rockSpawnAttempts;
	[Space(10)]
	public Mesh rockMesh;
	public Material rockMaterial;
	public Material plantMaterial;
	public Mesh groundMesh;
	public Material groundMaterial;
	public Mesh storeMesh;
	public Material storeMaterial;
	public AnimationCurve soldPlantYCurve;
	public AnimationCurve soldPlantXZScaleCurve;
	public AnimationCurve soldPlantYScaleCurve;


	public static GroundState[,] groundStates;
	public static List<Rock> rocks;
	[SerializeField]
	public static int rockCount;
	public static Rock[,] tileRocks;
	public static Dictionary<int,List<Plant>> plants;
	public static Plant[,] tilePlants;
	public static List<List<Matrix4x4>> rockMatrices;
	static List<int> plantSeeds;
	public static Dictionary<int,List<List<Matrix4x4>>> plantMatrices;
	Matrix4x4[][] groundMatrices;
	static float[][] tilledProperties;
	MaterialPropertyBlock[] groundMatProps;
	MaterialPropertyBlock plantMatProps;
	float[] plantGrowthProperties;
	public static bool[,] storeTiles;
	Matrix4x4[] storeMatrices;

	static List<Plant> pooledPlants;
	static List<Plant> soldPlants;
	static List<float> soldPlantTimers;

	public static Farm instance;
	public static int seedOffset;
	static int moneyForFarmers;
	static int moneyForDrones;

	public const int instancesPerBatch = 1023;

	void TrySpawnRock() {
		int width = Random.Range(0,4);
		int height = Random.Range(0,4);
		int rockX = Random.Range(0,mapSize.x - width);
		int rockY = Random.Range(0,mapSize.y - height);
		RectInt rect = new RectInt(rockX,rockY,width,height);

		bool blocked = false;
		for (int x = rockX; x <= rockX + width; x++) {
			for (int y = rockY; y <= rockY + height; y++) {
				if (tileRocks[x,y]!=null || storeTiles[x,y]) {
					blocked = true;
					break;
				}
			}
			if (blocked) break;
		}
		if (blocked == false) {
			Rock rock = new Rock(rect);
			rocks.Add(rock);
			if (rockMatrices[rockMatrices.Count-1].Count==instancesPerBatch) {
				rockMatrices.Add(new List<Matrix4x4>());
			}
			rock.batchNumber = rockMatrices.Count - 1;
			rock.batchIndex = rockMatrices[rock.batchNumber].Count;
			rockMatrices[rockMatrices.Count-1].Add(rock.matrix);

			for (int x = rockX; x <= rockX + width; x++) {
				for (int y = rockY; y <= rockY + height; y++) {
					tileRocks[x,y] = rock;
				}
			}
		}
	}

	public static void DeleteRock(Rock rock) {
		int index = rocks.IndexOf(rock);
		if (index != -1) {
			rocks.RemoveAt(index);
			rock.matrix.m00 = 0f;
			rock.matrix.m11 = 0f;
			rock.matrix.m22 = 0f;
			rockMatrices[rock.batchNumber][rock.batchIndex] = rock.matrix;
			for (int x = rock.rect.min.x; x <= rock.rect.max.x; x++) {
				for (int y = rock.rect.min.y; y <= rock.rect.max.y; y++) {
					tileRocks[x,y] = null;
				}
			}
			rockCount = rocks.Count;
		}
	}

	public static void TillGround(int x, int y) {
		groundStates[x,y] = GroundState.Tilled;
		int index = y * instance.mapSize.x + x;
		tilledProperties[index / instancesPerBatch][index % instancesPerBatch] = Random.Range(.8f,1f);
	}

	public static void RegisterSeed(int seed) {
		plantSeeds.Add(seed);
		plantMatrices.Add(seed,new List<List<Matrix4x4>>(Mathf.CeilToInt(instance.mapSize.x * instance.mapSize.y / (float)instancesPerBatch)));
		plantMatrices[seed].Add(new List<Matrix4x4>(instancesPerBatch));
		plants.Add(seed,new List<Plant>(instance.mapSize.x*instance.mapSize.y));
	}

	public static void SpawnPlant(int x, int y, int seed) {
		Plant plant = pooledPlants[pooledPlants.Count-1];
		pooledPlants.RemoveAt(pooledPlants.Count - 1);
		plant.Init(x,y,seed);
		plant.index = plants[seed].Count;
		plants[seed].Add(plant);
		tilePlants[x,y] = plant;
		groundStates[x,y] = GroundState.Plant;

		List<List<Matrix4x4>> matrices = plantMatrices[seed];

		if (matrices[matrices.Count-1].Count==instancesPerBatch) {
			matrices.Add(new List<Matrix4x4>(instancesPerBatch));
		}
		matrices[matrices.Count - 1].Add(plant.matrix);
	}
	public static void HarvestPlant(int x, int y) {
		tilePlants[x,y].harvested=true;
		tilePlants[x,y] = null;
		groundStates[x,y] = GroundState.Tilled;
	}
	public static void SellPlant(Plant plant, int storeX, int storeY) {
		plant.x = storeX;
		plant.y = storeY;
		soldPlants.Add(plant);
		soldPlantTimers.Add(0f);
		moneyForFarmers++;
		if (moneyForFarmers>=10) {
			if (FarmerManager.farmerCount < FarmerManager.instance.maxFarmerCount) {
				FarmerManager.SpawnFarmer(storeX,storeY);
				moneyForFarmers -= 10;
			}
			if (FarmerManager.farmerCount == FarmerManager.instance.maxFarmerCount / 2)
				Debug.Log("Max farmers halfway reached");
			if (FarmerManager.farmerCount == FarmerManager.instance.maxFarmerCount -1)
				Debug.Log("Max farmers halfway reached");
		}
		moneyForDrones++;
		if (moneyForDrones>=50) {
			for (int i = 0; i < 5; i++) {
				if (DroneManager.droneCount < DroneManager.instance.maxDroneCount) {
					DroneManager.SpawnDrone(storeX,storeY);
				}
				if (DroneManager.droneCount == DroneManager.instance.maxDroneCount/2)
					Debug.Log("Max drones halfway reached");
				if (DroneManager.droneCount == DroneManager.instance.maxDroneCount -1)
					Debug.Log("Max drones reached");
			}
			moneyForDrones -= 50;
		}
	}
	public static void DeletePlant(Plant plant) {
		pooledPlants.Add(plant);

		List<Plant> plantList = plants[plant.seed];
		plantList[plant.index] = plantList[plantList.Count - 1];
		plantList[plant.index].index = plant.index;
		plantList.RemoveAt(plantList.Count - 1);

		List<List<Matrix4x4>> matrices = plantMatrices[plant.seed];
		List<Matrix4x4> lastBatch = matrices[matrices.Count - 1];
		if (lastBatch.Count == 0) {
			matrices.RemoveAt(matrices.Count - 1);
			lastBatch = matrices[matrices.Count - 1];
		}

		matrices[plant.index / instancesPerBatch][plant.index % instancesPerBatch] = lastBatch[lastBatch.Count - 1];
		lastBatch.RemoveAt(lastBatch.Count - 1);
	}

	public static bool IsHarvestable(int x,int y) {
		Plant plant = tilePlants[x,y];
		return plant != null && plant.growth >= 1f;
	}
	public static bool IsHarvestableAndUnreserved(int x, int y) {
		Plant plant = tilePlants[x,y];
		return plant != null && plant.growth >= 1f && plant.reserved == false;
	}

	public static bool IsBlocked(Vector2Int tile) {
		return IsBlocked(tile.x,tile.y);
	}
	public static bool IsBlocked(int x, int y) {
		if (x<0 || y<0 || x>=instance.mapSize.x || y>=instance.mapSize.y) {
			return true;
		}

		if (tileRocks[x,y]!=null) {
			return true;
		} else {
			return false;
		}
	}
	
	Matrix4x4 GroundMatrix(int x, int y) {
		Vector3 pos = new Vector3(x + .5f,0f,y + .5f);
		float zRot = Random.Range(0,2) * 180f;
		return Matrix4x4.TRS(pos,Quaternion.Euler(90f,0f,zRot),Vector3.one);
	}

	void Awake () {
		instance = this;

		moneyForFarmers = 5;
		moneyForDrones = 0;

		Pathing.Init();
		seedOffset = Random.Range(int.MinValue,int.MaxValue);

		rocks = new List<Rock>();
		plants = new Dictionary<int,List<Plant>>();
		rockMatrices = new List<List<Matrix4x4>>();
		rockMatrices.Add(new List<Matrix4x4>());
		plantMatrices = new Dictionary<int,List<List<Matrix4x4>>>();
		plantSeeds = new List<int>();
		plantMatProps = new MaterialPropertyBlock();
		plantGrowthProperties = new float[instancesPerBatch];
		soldPlants = new List<Plant>(100);
		soldPlantTimers = new List<float>(100);
		
		int tileCount = mapSize.x * mapSize.y;
		pooledPlants = new List<Plant>(tileCount);
		for (int i=0;i<tileCount;i++) {
			pooledPlants.Add(new Plant());
		}

		groundStates = new GroundState[mapSize.x,mapSize.y];
		tileRocks = new Rock[mapSize.x,mapSize.y];
		tilePlants = new Plant[mapSize.x,mapSize.y];
		storeTiles = new bool[mapSize.x,mapSize.y];
		storeMatrices = new Matrix4x4[storeCount];
		groundMatrices = new Matrix4x4[Mathf.CeilToInt((float)tileCount / instancesPerBatch)][];
		groundMatProps = new MaterialPropertyBlock[groundMatrices.Length];
		tilledProperties = new float[groundMatrices.Length][];
		for (int i=0;i<groundMatrices.Length;i++) {
			groundMatProps[i] = new MaterialPropertyBlock();
			if (i<groundMatrices.Length-1) {
				groundMatrices[i] = new Matrix4x4[instancesPerBatch];
			} else {
				groundMatrices[i] = new Matrix4x4[tileCount-i*instancesPerBatch];
			}
			tilledProperties[i] = new float[groundMatrices[i].Length];
		}
		for (int y=0;y<mapSize.y;y++) {
			for (int x=0;x<mapSize.x;x++) {
				groundStates[x,y] = GroundState.Default;
				int index = y * mapSize.x + x;
				groundMatrices[index / instancesPerBatch][index % instancesPerBatch]=GroundMatrix(x,y);
				tilledProperties[index / instancesPerBatch][index % instancesPerBatch] = Random.value*.2f;
			}
		}

		for (int i=0;i<tilledProperties.Length;i++) {
			groundMatProps[i].SetFloatArray("_Tilled",tilledProperties[i]);
		}

		int spawnedStores = 0;

		while (spawnedStores<storeCount) {
			int x = Random.Range(0,mapSize.x);
			int y = Random.Range(0,mapSize.y);
			if (storeTiles[x,y]==false) {
				storeTiles[x,y] = true;
				storeMatrices[spawnedStores] = Matrix4x4.TRS(new Vector3(x+.5f,.6f,y+.5f),Quaternion.identity,new Vector3(1f,.6f,1f));
				spawnedStores++;
			}
		}

		for (int i=0;i<rockSpawnAttempts;i++) {
			TrySpawnRock();
		}
	}

	Camera cam;
	void Update () {

		if (Time.frameCount == 1 || Time.frameCount % 100 == 0)
        {
			Debug.Log("Frame:" + Time.frameCount + " Farmers " + FarmerManager.farmerCount + " | Drone Count " + DroneManager.droneCount + " | Rock Count " + rockCount + " | Plants " + plantSeeds.Count);
        }
		/*
		if (cam==null) {
			cam = Camera.main;
		}
		Ray ray = cam.ScreenPointToRay(Input.mousePosition);
		Plane groundPlane = new Plane(Vector3.up,Vector3.zero);
		float dist;
		if (groundPlane.Raycast(ray, out dist)) {
			Vector3 mousePos = ray.GetPoint(dist);
			Debug.DrawRay(mousePos,Vector3.up * 5f,Color.red);
			int x = Mathf.FloorToInt(mousePos.x);
			int y = Mathf.FloorToInt(mousePos.z);
			if (x>=0 && y>=0 && x<mapSize.x && y<mapSize.y) {
				if (Input.GetKey(KeyCode.Mouse0)) {
					if (groundStates[x,y]==GroundState.Tilled) {
						Plant plant = tilePlants[x,y];
						HarvestPlant(x,y);
						DeletePlant(plant);
					}
				}
			}
		}*/

		float smooth = 1f - Mathf.Pow(.1f,Time.deltaTime);
		for (int i=0;i<soldPlants.Count;i++) {
			Plant plant = soldPlants[i];
			soldPlantTimers[i] += Time.deltaTime;
			float t = soldPlantTimers[i];
			float y = soldPlantYCurve.Evaluate(t);
			float x = soldPlants[i].x+.5f;
			float z = soldPlants[i].y+.5f;
			float scaleXZ = soldPlantXZScaleCurve.Evaluate(t);
			float scaleY = soldPlantYScaleCurve.Evaluate(t);
			plant.EaseToWorldPosition(x,y,z,smooth);
			Vector3 pos = new Vector3(plant.matrix.m03,plant.matrix.m13,plant.matrix.m23);
			plant.matrix = Matrix4x4.TRS(pos,plant.rotation,new Vector3(scaleXZ,scaleY,scaleXZ));
			plant.ApplyMatrixToFarm();
			if (t>=1f) {
				DeletePlant(plant);
				soldPlants.RemoveAt(i);
				soldPlantTimers.RemoveAt(i);
				i--;
			}
		}


		Graphics.DrawMeshInstanced(storeMesh,0,storeMaterial,storeMatrices);
		for (int i = 0; i < rockMatrices.Count; i++) {
			Graphics.DrawMeshInstanced(rockMesh,0,rockMaterial,rockMatrices[i]);
		}
		for (int i=0;i<groundMatrices.Length;i++) {
			groundMatProps[i].SetFloatArray("_Tilled",tilledProperties[i]);
			Graphics.DrawMeshInstanced(groundMesh,0,groundMaterial,groundMatrices[i],groundMatrices[i].Length,groundMatProps[i]);
		}
		for (int i = 0; i < plantSeeds.Count; i++) {
			int seed = plantSeeds[i];
			Mesh plantMesh = Plant.meshLookup[seed];

			List<Plant> plantList = plants[seed];

			List<List<Matrix4x4>> matrices = plantMatrices[seed];
			for (int j = 0; j < matrices.Count; j++) {
				for (int k = 0; k < matrices[j].Count; k++) {
					Plant plant = plantList[j * instancesPerBatch + k];
					plant.growth = Mathf.Min(plant.growth + Time.deltaTime/10f,1f);
					plantGrowthProperties[k] = plant.growth;
				}
				plantMatProps.SetFloatArray("_Growth",plantGrowthProperties);
				Graphics.DrawMeshInstanced(plantMesh,0,plantMaterial,matrices[j],plantMatProps);
			}
		}
	}
}