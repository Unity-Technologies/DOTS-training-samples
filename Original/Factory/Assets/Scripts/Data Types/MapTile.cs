using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MapTile {
	public int moveCost;
	public bool isResourceSpawner;
	public PathType pathType;

	public MapTile(int cost,PathType type) {
		moveCost = cost;
		isResourceSpawner = false;
		pathType = type;
	}
}
