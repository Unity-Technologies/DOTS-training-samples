using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct ComponentBase : IComponentData
{
	public float Value;
}