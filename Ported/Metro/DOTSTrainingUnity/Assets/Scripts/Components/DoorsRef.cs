using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct DoorsRef : IComponentData
{
    public Entity doorEntLeft;
	public Entity doorEntRight;
}
