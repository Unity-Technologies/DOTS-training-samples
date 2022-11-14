using Unity.Entities;
using Unity.Mathematics;

public struct Bee : IComponentData
{
   public float3 Position;
   public float3 Velocity;
   public int Team;
   public float3 Scale;
   public float4 Color;
}
