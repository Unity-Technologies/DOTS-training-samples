using Unity.Entities;

[GenerateAuthoringComponent]
public struct PrefabSet : IComponentData
{
	public Entity ResourcePrefab;
	public Entity YellowBee;
	public Entity BlueBee;
	public Entity Particle;
}