using Unity.Entities;

using UnityMesh = UnityEngine.Mesh;
using UnityMaterial = UnityEngine.Material;

[GenerateAuthoringComponent]
public struct ParticleSettings : IComponentData
{
    public Entity Particle;
}