using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct BeeSpawner : IComponentData
{
	public Entity BeePrefab_TeamA;
	public Entity BeePrefab_TeamB;
	public byte Count;
}
