using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct AntMovementParameters : IComponentData
{
	public float randomWeight;
	public float pheromoneWeight;
	public float goalWeight;
	public float homeWeight;
	public bool debug;
}
