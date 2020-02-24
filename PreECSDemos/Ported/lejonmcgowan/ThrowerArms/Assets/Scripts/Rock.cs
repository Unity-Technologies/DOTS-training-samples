using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock {

	public enum State {
		Conveyor,
		Held,
		Thrown
	}

	public Vector3 position;
	public State state;
	public Vector3 velocity;
	public float size;
	public float targetSize;
	public bool reserved;
}
