using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Ant {
	public Vector2 position;
	public float facingAngle;
	public float speed;
	public bool holdingResource;
	public float brightness;

	public Ant(Vector2 pos) {
		position = pos;
		facingAngle = UnityEngine.Random.value * math.PI * 2f;
		speed = 0f;
		holdingResource = false;
		brightness = UnityEngine.Random.Range(.75f,1.25f);
	}
}
