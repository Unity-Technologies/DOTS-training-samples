using Components;
using DefaultNamespace;
using Unity.Entities;
using Unity.Mathematics;

public struct Bee : IComponentData
{
   public Team Team;
   public float3 Scale;
   public float2 Size;
   public Entity EntityTarget;
   public Beehaviors State;
}
