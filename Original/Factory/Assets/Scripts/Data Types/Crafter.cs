using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crafter {
	public Vector2Int position;
	public int requiredResourceCount;
	public int inventory;
	public int workerCount;
	public FlowField navigator;
	public bool destroyed = false;

	public Crafter(Vector2Int tile) {
		position = tile;
		requiredResourceCount = 10;
		inventory = 0;
		workerCount = 0;
	}
}
