using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

public class ClothRenderingSystem : SystemBase
{
	protected override void OnUpdate()
	{
		Entities.WithStructuralChanges().ForEach((in Entity entity, in ClothMesh clothMesh, in RenderMesh renderMesh) =>
		{
			if (renderMesh.mesh != clothMesh.mesh)
			{
				var renderMeshCopy = renderMesh;
				{
					renderMeshCopy.mesh = clothMesh.mesh;
					EntityManager.SetSharedComponentData(entity, renderMeshCopy);
				}
			}

			clothMesh.mesh.SetVertices(clothMesh.vertexPosition);
		}
		).WithoutBurst().Run();
	}
}
