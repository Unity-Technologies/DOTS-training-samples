using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

class ParticleAuthoring : MonoBehaviour
{
}

class ParticleBaker : Baker<ParticleAuthoring>
{
    public override void Bake(ParticleAuthoring authoring)
    {
        AddComponent(new Particle());
        AddComponent(new PostTransformScale()
        {
            Value = float3x3.Scale(1f, 1f, 1f)
        });    
    }
}