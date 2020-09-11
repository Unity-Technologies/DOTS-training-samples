using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class ClothMeshDisposer : SystemBase
{
	protected override void OnUpdate()
	{
		//Entities
		//	.WithAll<ClothMesh>()
		//	.WithNone<ClothMeshToken>()
		//	.ForEach((in ClothMesh clothMesh) =>
		//	{
		//		clothMesh.Dispose();
		//		Mesh.Destroy(clothMesh.mesh);
		//	}
		//).WithoutBurst().Run();
	}
}
