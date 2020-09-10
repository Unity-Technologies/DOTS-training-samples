using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct SmokeSpawner : IComponentData
{
	public Entity Smoke;
	public byte Count;
}
