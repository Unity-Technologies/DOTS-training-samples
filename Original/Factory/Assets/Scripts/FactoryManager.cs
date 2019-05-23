using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryManager : MonoBehaviour {
	public TextAsset mapData;
	[Space(10)]
	public Mesh wallMesh;
	public Material wallMaterial;
	public Material crafterPathMaterial;
	public Material resourcePathMaterial;
	public Material crafterMaterial;
	public Material resourceSpawnerMaterial;

	public static ToolType currentTool;

	[System.NonSerialized]
	public int mapWidth;
	[System.NonSerialized]
	public int mapHeight;

	List<Vector2Int> walls;
	List<Vector2Int> crafterPathTiles;
	List<Vector2Int> resourcePathTiles;
	List<Matrix4x4> wallMatrices;
	List<Matrix4x4> crafterPathMatrices;
	List<Matrix4x4> resourcePathMatrices;
	List<Crafter> crafters;
	List<Matrix4x4> crafterMatrices;
	List<ResourceSpawner> resourceSpawners;
	List<Matrix4x4> resourceSpawnerMatrices;
	[System.NonSerialized]
	public Transform factoryMarker;
	BotManager botManager;
	List<FlowField> flowFields;
	int flowFieldTicker = 0;

	[System.NonSerialized]
	public FlowField resourceNavigator;

	Camera mainCam;

	public Map map;
	
	Matrix4x4 GetTileMatrix(Vector2Int tile) {
		Vector3 position = factoryMarker.TransformPoint(new Vector3(tile.x, 0f, tile.y));
		Quaternion rotation = Quaternion.Euler(90f,0f,0f);
		return Matrix4x4.TRS(position,rotation,factoryMarker.localScale);
	}

	public Crafter GetRequestingCrafter() {
		Crafter output = null;
		float lowestStock = float.MaxValue;
		for (int i=0;i<crafters.Count;i++) {
			if (crafters[i].inventory+crafters[i].workerCount < lowestStock) {
				lowestStock = crafters[i].inventory+crafters[i].workerCount;
				output = crafters[i];
			}
		}
		return output;
	}

	void EditTile(Vector2Int tile,ToolType tool,bool deleteOldTile) {
		if (tool == ToolType.SpawnAgents) {
			return;
		}

		if (map.IsInsideMap(tile)==false) {
			return;
		}

		if (deleteOldTile) {
			MapTile currentTile = map.tiles[tile.x,tile.y];

			if (currentTile.isResourceSpawner) {
				resourceNavigator.targets.Remove(tile);
				for (int i = 0; i < resourceSpawners.Count; i++) {
					if (resourceSpawners[i].position == tile) {
						resourceSpawners[i].destroyed = true;
						resourceSpawners.RemoveAt(i);
						resourceSpawnerMatrices.RemoveAt(i);
						break;
					}
				}
			} else if (currentTile.moveCost == map.maxCost) {
				int index = walls.IndexOf(tile);
				walls.RemoveAt(index);
				wallMatrices.RemoveAt(index);
			} else if (currentTile.pathType == PathType.Crafter) {
				int index = crafterPathTiles.IndexOf(tile);
				crafterPathTiles.RemoveAt(index);
				crafterPathMatrices.RemoveAt(index);
				for (int i = 0; i < flowFields.Count; i++) {
					if (flowFields[i].targets.Contains(tile)) {
						flowFields.RemoveAt(i);
						break;
					}
				}
			} else if (currentTile.pathType == PathType.Resource) {
				int index = resourcePathTiles.IndexOf(tile);
				resourcePathTiles.RemoveAt(index);
				resourcePathMatrices.RemoveAt(index);

				for (int i = 0; i < flowFields.Count; i++) {
					if (flowFields[i].targets.Contains(tile)) {
						Debug.Log("removing flow target");
						flowFields[i].targets.Remove(tile);
					}
				}
			} else {
				// process of elimination: this tile must be a crafter

				for (int i=0;i<crafters.Count;i++) {
					if (crafters[i].position==tile) {
						crafters[i].destroyed = true;
						crafters.RemoveAt(i);
						crafterMatrices.RemoveAt(i);
						break;
					}
				}
			}
		}

		MapTile newTile = new MapTile();
		newTile.pathType = PathType.Default;
		newTile.moveCost = 1;

		if (tool == ToolType.Empty) {
			// nothing to see here
		} else if (tool==ToolType.Wall) {
			newTile.moveCost = map.maxCost;
			walls.Add(tile);
			wallMatrices.Add(GetTileMatrix(tile));
		} else if (tool == ToolType.Crafter) {
			Crafter crafter = new Crafter(tile);
			crafter.navigator = new FlowField(map,tile,PathType.Crafter);
			crafters.Add(crafter);
			crafterMatrices.Add(GetTileMatrix(tile));
			flowFields.Add(crafter.navigator);
		} else if (tool==ToolType.Resource) {
			ResourceSpawner spawner = new ResourceSpawner(tile);
			newTile.isResourceSpawner = true;
			resourceNavigator.targets.Add(tile);
			resourceSpawners.Add(spawner);
			resourceSpawnerMatrices.Add(GetTileMatrix(tile));
		} else if (tool==ToolType.CrafterPath) {
			newTile.pathType = PathType.Crafter;
			crafterPathTiles.Add(tile);
			crafterPathMatrices.Add(GetTileMatrix(tile));
		} else if (tool==ToolType.ResourcePath) {
			newTile.pathType = PathType.Resource;
			resourcePathTiles.Add(tile);
			resourcePathMatrices.Add(GetTileMatrix(tile));
		}

		map.tiles[tile.x,tile.y] = newTile;
	}

	void Awake () {
		int i, j;

		mainCam = Camera.main;

		string mapString = mapData.text.Trim();
		mapString=mapString.Replace(",","");
		string[] rows = mapString.Split("\n"[0]);

		for (i=0;i<rows.Length;i++) {
			rows[i] = rows[i].Trim();
		}

		mapWidth = rows[0].Length;
		mapHeight = rows.Length;

		factoryMarker = new GameObject("Factory Marker").transform;
		factoryMarker.position = new Vector3(-(mapWidth - 1) * .5f,0f,-(mapHeight - 1) * .5f);

		Camera.main.orthographicSize = mapHeight * .5f;

		map = new Map(mapWidth,mapHeight,mapString);
		walls = new List<Vector2Int>();
		crafterPathTiles = new List<Vector2Int>();
		resourcePathTiles = new List<Vector2Int>();

		crafters = new List<Crafter>();
		resourceSpawners = new List<ResourceSpawner>();
		flowFields = new List<FlowField>();

		crafterMatrices = new List<Matrix4x4>();
		resourceSpawnerMatrices = new List<Matrix4x4>();
		resourceNavigator=new FlowField(map,new List<Vector2Int>(),PathType.Resource);
		flowFields.Add(resourceNavigator);

		wallMatrices = new List<Matrix4x4>();
		crafterPathMatrices = new List<Matrix4x4>();
		resourcePathMatrices = new List<Matrix4x4>();

		for (i = 0; i < mapWidth; i++) {
			for (j=0;j<mapHeight;j++) {
				char character = rows[mapHeight - 1 - j][i];
				ToolType tileTool = ToolType.Empty;
				if (character == "0"[0]) {
					tileTool = ToolType.Empty;
				} else if (character=="1"[0]) {
					tileTool = ToolType.Wall;
				} else if (character=="X"[0]) {
					tileTool = ToolType.Crafter;
				} else if (character=="Z"[0]) {
					tileTool = ToolType.Resource;
				} else if (character=="="[0]) {
					tileTool = ToolType.CrafterPath;
				} else if (character=="-"[0]) {
					tileTool = ToolType.ResourcePath;
				}
				EditTile(new Vector2Int(i,j),tileTool,false);
			}
		}

		botManager = GetComponent<BotManager>();
		botManager.Init();
	}
	
	void Update () {
		if (Input.GetKey(KeyCode.Mouse0)) {
			Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
			mousePos = factoryMarker.InverseTransformPoint(mousePos);
			Vector2Int tile = new Vector2Int(Mathf.FloorToInt(mousePos.x + .5f),Mathf.FloorToInt(mousePos.z + .5f));

			if (currentTool == ToolType.SpawnAgents) {
				for (int i = 0; i < 3; i++) {
					botManager.TrySpawnBot(tile);
				}
			} else {
				EditTile(tile,currentTool,true);
			}
		}

		flowFields[flowFieldTicker].Generate();
		flowFieldTicker++;
		if (flowFieldTicker>=flowFields.Count) {
			flowFieldTicker = 0;
		}

		Graphics.DrawMeshInstanced(wallMesh,0,crafterPathMaterial,crafterPathMatrices);
		Graphics.DrawMeshInstanced(wallMesh,0,resourcePathMaterial,resourcePathMatrices);
		Graphics.DrawMeshInstanced(wallMesh,0,wallMaterial,wallMatrices);
		Graphics.DrawMeshInstanced(wallMesh,0,crafterMaterial,crafterMatrices);
		Graphics.DrawMeshInstanced(wallMesh,0,resourceSpawnerMaterial,resourceSpawnerMatrices);
	}
}
