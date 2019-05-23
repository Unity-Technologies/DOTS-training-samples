using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct FactoryBot {
	public Vector2 position;
	public float radius;
	public int hitCount;
	public FlowField navigator;
	public Crafter targetCrafter;
	public bool movingToResource;
	public bool holdingResource;

	public FactoryBot(Vector2Int myTile) {
		position = new Vector2(myTile.x + Random.Range(-.5f,.5f),myTile.y + Random.Range(-.5f,.5f));
		radius = .15f;
		hitCount = 0;
		navigator = null;
		targetCrafter = null;
		movingToResource = false;
		holdingResource = false;
	}
	public FactoryBot(Vector2 myPosition) {
		position = myPosition;
		radius = .15f;
		hitCount = 0;
		navigator = null;
		targetCrafter = null;
		movingToResource = false;
		holdingResource = false;
	}
}