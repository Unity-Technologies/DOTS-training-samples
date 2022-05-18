using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public struct Door : IComponentData
{
	public Entity Carriage;
	public float DoorState;
	public float3 StartPosition;
	public float3 EndPosition;
	public bool Open;
}