using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource {
	public Vector3 position;
	public bool stacked;
	public int stackIndex;
	public int gridX;
	public int gridY;
	public Bee holder;
	public Vector3 velocity;
	public bool dead;

	public Resource(Vector3 myPosition) {
		position = myPosition;
	}
}
