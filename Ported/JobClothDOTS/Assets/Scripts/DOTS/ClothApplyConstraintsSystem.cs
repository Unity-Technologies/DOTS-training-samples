//#define AVOID_NESTED_FOREACH
//#define RUN_ON_MAIN

using Unity.Entities;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(ClothApplyGravitySystem))]
public class ClothApplyConstraintsSystem : SystemBase
{
	protected override void OnUpdate()
	{
#if AVOID_NESTED_FOREACH

		var entityQuery = GetEntityQuery(
			ComponentType.ReadWrite<ClothMeshToken>(),
			ComponentType.ReadOnly<ClothMesh>()
		);

		var chunkArray = entityQuery.CreateArchetypeChunkArray(Unity.Collections.Allocator.TempJob);

		var typeHandleEntity = GetEntityTypeHandle();
		var typeHandleClothMesh = GetSharedComponentTypeHandle<ClothMesh>();
		var typeHandleClothMeshToken = GetComponentTypeHandle<ClothMeshToken>();

		for (int i = 0; i != chunkArray.Length; i++)
		{
			var chunk = chunkArray[i];
			var chunkEntities = chunk.GetNativeArray(typeHandleEntity);

			var clothMesh = chunk.GetSharedComponentData(typeHandleClothMesh, EntityManager);

			var clothMeshTokenEntity = chunkEntities[0];
			var clothMeshToken = EntityManager.GetComponentData<ClothMeshToken>(clothMeshTokenEntity);

			UnityEngine.Debug.Log("chunk " + i + ", clothMesh " + clothMesh.mesh.name);

			var vertexPosition = clothMesh.vertexPosition;
			var vertexInvMass = clothMesh.vertexInvMass;

			clothMeshToken.jobHandle = Entities.WithSharedComponentFilter(clothMesh).ForEach((in ClothEdge edge) =>
			{
				int index0 = edge.IndexPair.x;
				int index1 = edge.IndexPair.y;

				var p0 = vertexPosition[index0];
				var p1 = vertexPosition[index1];
				var w0 = vertexInvMass[index0];
				var w1 = vertexInvMass[index1];

				float3 r = p1 - p0;
				float rd = math.length(r);

				float delta = 1.0f - edge.Length / rd;
				float W_inv = delta / (w0 + w1);

				vertexPosition[index0] += r * (w0 * W_inv);
				vertexPosition[index1] -= r * (w1 * W_inv);
			}
			).Schedule(clothMeshToken.jobHandle);

			EntityManager.SetComponentData(clothMeshTokenEntity, clothMeshToken);
		}

#else

		Entities.ForEach((ref ClothMeshToken clothMeshToken, in ClothMesh clothMesh) =>
		{
			var vertexPosition = clothMesh.vertexPosition;
			var vertexInvMass = clothMesh.vertexInvMass;

#if RUN_ON_MAIN
			clothMeshToken.jobHandle.Complete();
			Entities.WithSharedComponentFilter(clothMesh).ForEach((in ClothEdge edge) =>
#else
			clothMeshToken.jobHandle = Entities.WithSharedComponentFilter(clothMesh).ForEach((in ClothEdge edge) =>
#endif
			{
				int index0 = edge.IndexPair.x;
				int index1 = edge.IndexPair.y;

				var p0 = vertexPosition[index0];
				var p1 = vertexPosition[index1];
				var w0 = vertexInvMass[index0];
				var w1 = vertexInvMass[index1];

				float3 r = p1 - p0;
				float rd = math.length(r);

				float delta = 1.0f - edge.Length / rd;
				float W_inv = delta / (w0 + w1);

				vertexPosition[index0] += r * (w0 * W_inv);
				vertexPosition[index1] -= r * (w1 * W_inv);
			}
#if RUN_ON_MAIN
			).Run();
#else
			).Schedule(clothMeshToken.jobHandle);
#endif

			//TODO: ScheduleParallel
		}
		).WithoutBurst().Run();

#endif
	}
}
