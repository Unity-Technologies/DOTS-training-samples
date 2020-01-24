using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Transforms;


[AlwaysUpdateSystem]
[UpdateInGroup(typeof(PresentationSystemGroup))]
public class ParticleRenderSystem : ComponentSystem
{
    EntityQuery m_ParticleMesh;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_ParticleMesh = GetEntityQuery(ComponentType.ReadOnly<ParticleMesh>());
    }
    protected override void OnUpdate()
    {
        var particleSetup = EntityManager.GetSharedComponentData<ParticleMesh>(m_ParticleMesh.GetSingletonEntity());

        Entities
            .ForEach((ref LocalToWorld localToWorld) =>
            {
                Graphics.DrawMesh(particleSetup.Mesh, localToWorld.Value, particleSetup.Material, 0);
            });
    }
}

