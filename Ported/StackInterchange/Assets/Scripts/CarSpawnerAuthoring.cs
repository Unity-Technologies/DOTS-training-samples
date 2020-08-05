using Unity.Entities;
using Unity.Mathematics;
 
[GenerateAuthoringComponent]
public struct CarSpawnerAuthoring : IComponentData
{
    public Entity CarPrefab;
    public int Count;
}
