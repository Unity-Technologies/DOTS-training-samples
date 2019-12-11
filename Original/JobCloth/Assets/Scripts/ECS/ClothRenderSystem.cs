using Unity.Entities;
using Unity.Jobs;
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

        Entities.WithoutBurst().ForEach((ClothComponent cloth, ref LocalToWorld localToWorld) =>
        {
            cloth.Mesh.SetVertices(cloth.CurrentClothPosition);

            Graphics.DrawMesh(cloth.Mesh, localToWorld.Value, cloth.Material, 0);
        }).Run();

        return inputDeps;
    }
    override protected void OnDestroy()
    {
        Entities.WithoutBurst().ForEach((ClothComponent cloth) =>
        {
            if (cloth.CurrentClothPosition.IsCreated) cloth.CurrentClothPosition.Dispose();
            if (cloth.PreviousClothPosition.IsCreated) cloth.PreviousClothPosition.Dispose();
            if (cloth.Forces.IsCreated) cloth.Forces.Dispose();
            if (cloth.ClothNormals.IsCreated) cloth.ClothNormals.Dispose();
        }).Run();
    }
}
