using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[ExecuteAlways]
[AlwaysUpdateSystem]
[UpdateInGroup(typeof(PresentationSystemGroup))]
[UpdateAfter(typeof(CollisionMesh_System))]
public class ClothRenderSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        inputDeps.Complete();

        Entities.WithoutBurst().ForEach((Entity entity, in ClothComponent cloth, in ClothRenderComponent renderCloth, in LocalToWorld localToWorld) =>
        {
            var vertices = EntityManager.GetBuffer<CurrentVertex>(entity);
            renderCloth.Mesh.SetVertices(vertices.Reinterpret<float3>().AsNativeArray());

            Graphics.DrawMesh(renderCloth.Mesh, localToWorld.Value, renderCloth.Material, 0);
        }).Run();

        return inputDeps;
    }
}
