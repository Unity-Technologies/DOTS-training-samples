using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct BloodSpawner : IComponentData
{
	public Entity blood;
	public byte Count;
}
