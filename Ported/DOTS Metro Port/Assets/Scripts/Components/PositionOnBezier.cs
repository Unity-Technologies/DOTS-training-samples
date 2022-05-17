using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct DistanceAlongBezier : IComponentData
{
	public float Distance;
	public Entity TrackEntity;
}
