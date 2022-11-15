using Components;
using Unity.Entities;
using Unity.Mathematics;

public struct Bee : IComponentData
{
   public float3 Position;
   public float3 Velocity;
   public Team Team;
   public float3 Scale;
   public float2 Size;
   public int EnemyTarget;
   public int ResourceTarget;
}
