using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct PositionOnBezier : IComponentData
{
	public float Position;
	public int BezierIndex;
}
