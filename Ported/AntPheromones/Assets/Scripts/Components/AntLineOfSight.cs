using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct AntLineOfSight : IComponentData
{
	public float DegreesToGoal;
}
