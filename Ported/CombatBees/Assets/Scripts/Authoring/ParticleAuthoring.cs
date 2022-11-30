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
    }
}