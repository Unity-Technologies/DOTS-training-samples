using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct Config : IComponentData
{
	public float ObstacleRadius;
	public float ObstaclesPerRing;
	public int ObstacleRingCount;
	public float MapSize;
}
