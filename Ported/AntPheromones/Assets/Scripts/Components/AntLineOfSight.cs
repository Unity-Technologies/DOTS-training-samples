using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct AntLineOfSight : IComponentData
{
	public float DegreesToFood;		// note that this assumes that the ant makes a beeline directly to the food
}
