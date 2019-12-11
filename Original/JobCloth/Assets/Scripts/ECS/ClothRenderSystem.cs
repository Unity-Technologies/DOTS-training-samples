using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

[ExecuteAlways]
[AlwaysUpdateSystem]
[UpdateInGroup(typeof(PresentationSystemGroup))]
[UpdateAfter(typeof(CollisionMeshJob))]
public class ClothRenderSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        inputDeps.Complete();

        Entities.WithoutBurst().ForEach((ClothComponent cloth, ref LocalToWorld localToWorld) =>
        {
            cloth.Mesh.SetVertices(cloth.CurrentClothPosition);

            Graphics.DrawMesh(cloth.Mesh, localToWorld.Value, cloth.Material, 0);
        }).Run();

        return inputDeps;
    }
}
