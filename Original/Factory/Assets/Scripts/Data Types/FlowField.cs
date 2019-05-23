using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowField {
	Map map;
	public List<Vector2Int> targets;
	public float[,] stepField;
	public Vector2[,] flowField;

	public int maxPathCost;
	public PathType pathType;

	Vector2Int[] moveDirs = new Vector2Int[] {new Vector2Int(0,1),
										  new Vector2Int(1,1),
										  new Vector2Int(1,0),
										  new Vector2Int(1,-1),
										  new Vector2Int(0,-1),
										  new Vector2Int(-1,-1),
										  new Vector2Int(-1,0),
										 new Vector2Int(-1,1)};

	List<Vector2Int> openSet;
	List<Vector2Int> nextSet;

	public FlowField(Map myMap,Vector2Int targetTile,PathType type=PathType.Default) {
		map = myMap;
		targets = new List<Vector2Int>();
		targets.Add(targetTile);
		pathType = type;
		Generate();
	}
	public FlowField(Map myMap,List<Vector2Int> targetTiles,PathType type=PathType.Default) {
		map = myMap;
		targets = targetTiles;
		pathType = type;
		Generate();
	}


	public void Generate() {
		int i, j, k;

		if (stepField == null) {
			stepField = new float[map.width,map.height];
			flowField = new Vector2[map.width,map.height];
		}

		if (openSet == null) {
			openSet = new List<Vector2Int>();
			nextSet = new List<Vector2Int>();
		} else {
			openSet.Clear();
			nextSet.Clear();
		}
		for (i = 0; i < targets.Count; i++) {
			openSet.Add(targets[i]);
		}
		for (i=0;i<map.width;i++) {
			for (j=0;j<map.height;j++) {
				stepField[i,j] = int.MaxValue;
				flowField[i,j] = Vector2.zero;
			}
		}
		
		for (i = 0; i < targets.Count; i++) {
			stepField[targets[i].x,targets[i].y] = 0;
		}

		while (openSet.Count>0) {
			for (j = 0; j < openSet.Count; j++) {
				Vector2Int tile = openSet[j];
				float existingCost = stepField[tile.x,tile.y];
				for (i = 0; i < moveDirs.Length; i++) {
					Vector2Int newTile = new Vector2Int(tile.x+moveDirs[i].x, tile.y+moveDirs[i].y);
					if (map.IsInsideMap(newTile)) {
						if (map.IsWall(newTile) == false) {
							Vector2Int checkTileA = new Vector2Int(tile.x + moveDirs[i].x,tile.y);
							Vector2Int checkTileB = new Vector2Int(tile.x,tile.y + moveDirs[i].y);
							if (map.IsWall(checkTileA) == false) {
								if (map.IsWall(checkTileB) == false) {
									float moveCost = map.tiles[newTile.x,newTile.y].moveCost;
									if (map.occupantTickers[newTile.x,newTile.y] == map.occupantTicker) {
										moveCost += map.occupants[newTile.x,newTile.y] * .5f;
									}
									if (pathType!=PathType.Default) {
										if (pathType==map.tiles[newTile.x,newTile.y].pathType) {
											moveCost *= .3f;
										}
									}
									float cost = moveCost * moveDirs[i].magnitude + existingCost;
									if (cost < stepField[newTile.x,newTile.y]) {
										stepField[newTile.x,newTile.y] = cost;
										nextSet.Add(newTile);
									}
								}
							}
						}
					}
				}
			}
			List<Vector2Int> temp = openSet;
			openSet = nextSet;
			nextSet = temp;
			nextSet.Clear();
		}

		for (i=0;i<map.width;i++) {
			for (j=0;j<map.height;j++) {
				Vector2Int tile = new Vector2Int(i,j);
				Vector2 flow = Vector2.zero;
				if (map.IsWall(tile) == false) {
					float myCost = stepField[i,j];
					float minNeighborCost = myCost;
					Vector2Int bestNeighbor = new Vector2Int(i,j);
					for (k = 0; k < moveDirs.Length; k++) {
						Vector2Int newTile = new Vector2Int(tile.x + moveDirs[k].x,tile.y + moveDirs[k].y);
						if (map.IsInsideMap(newTile)) {
							Vector2Int checkTileA = new Vector2Int(tile.x + moveDirs[k].x,tile.y);
							Vector2Int checkTileB = new Vector2Int(tile.x,tile.y + moveDirs[k].y);
							if (map.IsWall(checkTileA) == false) {
								if (map.IsWall(checkTileB) == false) {
									if (stepField[newTile.x,newTile.y] < minNeighborCost) {
										minNeighborCost = stepField[newTile.x,newTile.y];
										bestNeighbor = newTile;
									}
								}
							}
						}
					}

					Vector2Int intFlow = new Vector2Int(bestNeighbor.x - tile.x,bestNeighbor.y - tile.y);
					flow = new Vector2(intFlow.x,intFlow.y);
					if (flow.sqrMagnitude > 0f) {
						flow = flow.normalized;
					}
				}
				flowField[i,j] = flow;
			}
		}
	}
}
