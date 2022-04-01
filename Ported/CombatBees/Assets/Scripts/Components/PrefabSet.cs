using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct PrefabSet : IComponentData
{
	public Entity ResourcePrefab;
	public Entity YellowBee;
	public Entity BlueBee;
	//public Entity YellowBeeSpawner;
	//public Entity BlueBeeSpawner;
}
