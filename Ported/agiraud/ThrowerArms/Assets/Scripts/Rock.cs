using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RockState
{
    Conveyor,
    Held,
    Thrown
}

public class Rock {
	public Vector3 position;
	public RockState rockState;
	public Vector3 velocity;
	public float size;
	public float targetSize;
	public bool reserved;
}
