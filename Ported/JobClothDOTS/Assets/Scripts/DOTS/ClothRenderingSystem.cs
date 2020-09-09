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
		Entities.WithStructuralChanges().ForEach((ref LocalToWorld localToWorld, in Entity entity, in ClothMesh clothMesh, in ClothMeshToken clothMeshToken, in RenderMesh renderMesh) =>
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

			localToWorld.Value = float4x4.identity;
		}
		).WithoutBurst().Run();
	}
}
