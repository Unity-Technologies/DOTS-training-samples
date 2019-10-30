using Unity.Entities;
using Unity.Mathematics;



[GenerateAuthoringComponent]
public struct Spawner : IComponentData
{
    public Entity Prefab; //The Prefab sould have Velocity, Gravity or whatever we need to be sure it behaves as it is supposet it should be
    public float3 Position;
    public int Amount;
    public float Speed;
}
