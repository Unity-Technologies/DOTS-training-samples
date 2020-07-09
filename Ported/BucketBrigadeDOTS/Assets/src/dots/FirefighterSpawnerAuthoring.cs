using Unity.Entities;
 
[GenerateAuthoringComponent]
public struct FirefighterSpawner : IComponentData
{
    public Entity Prefab;
    public int Count;
}