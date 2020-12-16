using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct PheromoneDecayRate : IComponentData
{
	public float pheromoneDecayRate;
}
