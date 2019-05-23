using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawner {
	public Vector2Int position;
	public bool destroyed = false;

	public ResourceSpawner(Vector2Int tile) {
		position = tile;
	}
}
