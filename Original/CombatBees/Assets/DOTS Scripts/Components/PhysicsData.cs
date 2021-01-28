using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct PhysicsData : IComponentData
{
    public float3 a;
    public float3 v;
    public float3 kineticEnergyPreserved;
    [Range(0f,1f)] 
    public float damping;
}