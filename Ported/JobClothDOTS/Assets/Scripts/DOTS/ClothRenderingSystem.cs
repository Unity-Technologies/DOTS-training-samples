using Unity.Entities;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Rendering;
using Unity.Transforms;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public class ClothRenderingSystem : SystemBase
{
	protected override void OnUpdate()
	{
		Entities.WithStructuralChanges().ForEach((ref LocalToWorld localToWorld, ref RenderBounds localRenderBounds, in Entity entity, in ClothMesh clothMesh, in ClothMeshToken clothMeshToken, in RenderMesh renderMesh) =>
		{
			if (renderMesh.mesh != clothMesh.mesh)
			{
				var renderMeshCopy = renderMesh;
				{
					renderMeshCopy.mesh = clothMesh.mesh;
					EntityManager.SetSharedComponentData(entity, renderMeshCopy);
				}
			}

			clothMeshToken.jobHandle.Complete();
			clothMesh.mesh.SetVertices(clothMesh.vertexPosition);

			var bounds = clothMesh.mesh.bounds;

			localToWorld.Value = float4x4.identity;
			localRenderBounds.Value = new AABB
			{
				Center = new float3(0f,0f,0f),
				Extents = new float3(1f, 1f, 1f),
			};
		}
		).WithoutBurst().Run();
	}
}
