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

        Entities.WithoutBurst().ForEach((ClothComponent cloth, ClothRenderComponent renderCloth, ref LocalToWorld localToWorld) =>
        {
            renderCloth.Mesh.SetVertices(cloth.CurrentClothPosition);

            Graphics.DrawMesh(renderCloth.Mesh, localToWorld.Value, renderCloth.Material, 0);
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
