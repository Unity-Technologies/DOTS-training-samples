//#define USE_CHUNK_ITERATION
#define USE_ATOMICS

using System.Threading;
using Unity.Entities;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Burst;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(ClothApplyGravitySystem))]
public class ClothApplyConstraintsSystem : SystemBase
{
#if USE_ATOMICS
	const int Fixed32FractionalBits = 14;// => 1 / (1 << 14) = 0.00006103515625

	const float ScaleFloatToFixed32 = (1 << Fixed32FractionalBits);
	const float ScaleFixed32ToFloat = (1.0f / (1 << Fixed32FractionalBits));

	[BurstCompile]
	struct ApplyDeltaJob : IJobParallelFor
	{
		[NoAlias] public NativeArray<float3> vertexPosition;
		[NoAlias, ReadOnly] public NativeArray<int> vertexPositionDeltaX;
		[NoAlias, ReadOnly] public NativeArray<int> vertexPositionDeltaY;
		[NoAlias, ReadOnly] public NativeArray<int> vertexPositionDeltaZ;
		[NoAlias, ReadOnly] public NativeArray<int> vertexPositionDeltaW;

		public void Execute(int i)
		{
			float3 delta = new float3
			{
				x = (ScaleFixed32ToFloat * vertexPositionDeltaX[i]) / vertexPositionDeltaW[i],
				y = (ScaleFixed32ToFloat * vertexPositionDeltaY[i]) / vertexPositionDeltaW[i],
				z = (ScaleFixed32ToFloat * vertexPositionDeltaZ[i]) / vertexPositionDeltaW[i],
			};

			vertexPosition[i] += delta;
		}
	}
#endif

	protected override void OnUpdate()
	{
#if !USE_CHUNK_ITERATION

		Entities.ForEach((ref ClothMeshToken clothMeshToken, in ClothMesh clothMesh) =>
		{
			var vertexPosition = clothMesh.vertexPosition;
			var vertexInvMass = clothMesh.vertexInvMass;

#if USE_ATOMICS
			unsafe
#endif
			{
#if USE_ATOMICS
				var deltaCount = clothMesh.vertexPosition.Length;
				var deltaX = (int*)clothMesh.vertexPositionDeltaX.GetUnsafePtr();
				var deltaY = (int*)clothMesh.vertexPositionDeltaY.GetUnsafePtr();
				var deltaZ = (int*)clothMesh.vertexPositionDeltaZ.GetUnsafePtr();
				var deltaW = (int*)clothMesh.vertexPositionDeltaW.GetUnsafePtr();

				clothMeshToken.jobHandle = Job
					.WithNativeDisableUnsafePtrRestriction(deltaX)
					.WithNativeDisableUnsafePtrRestriction(deltaY)
					.WithNativeDisableUnsafePtrRestriction(deltaZ)
					.WithNativeDisableUnsafePtrRestriction(deltaW)
					.WithCode(() =>
				{
					UnsafeUtility.MemClear(deltaX, deltaCount * sizeof(int));
					UnsafeUtility.MemClear(deltaY, deltaCount * sizeof(int));
					UnsafeUtility.MemClear(deltaZ, deltaCount * sizeof(int));
					UnsafeUtility.MemClear(deltaW, deltaCount * sizeof(int));
				}
				).Schedule(clothMeshToken.jobHandle);
#endif

				clothMeshToken.jobHandle = Entities
#if USE_ATOMICS
					.WithNativeDisableUnsafePtrRestriction(deltaX)
					.WithNativeDisableUnsafePtrRestriction(deltaY)
					.WithNativeDisableUnsafePtrRestriction(deltaZ)
					.WithNativeDisableUnsafePtrRestriction(deltaW)
#endif
					.WithSharedComponentFilter(clothMesh).ForEach((in ClothEdge edge) =>
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

#if USE_ATOMICS
					float3 delta0 = r * (w0 * W_inv);
					float3 delta1 = r * (w1 * W_inv);

					Interlocked.Add(ref deltaX[index0], (int)math.round(ScaleFloatToFixed32 * delta0.x));
					Interlocked.Add(ref deltaY[index0], (int)math.round(ScaleFloatToFixed32 * delta0.y));
					Interlocked.Add(ref deltaZ[index0], (int)math.round(ScaleFloatToFixed32 * delta0.z));

					Interlocked.Add(ref deltaX[index1], (int)math.round(-ScaleFloatToFixed32 * delta1.x));
					Interlocked.Add(ref deltaY[index1], (int)math.round(-ScaleFloatToFixed32 * delta1.y));
					Interlocked.Add(ref deltaZ[index1], (int)math.round(-ScaleFloatToFixed32 * delta1.z));

					Interlocked.Increment(ref deltaW[index0]);
					Interlocked.Increment(ref deltaW[index1]);
#else
					vertexPosition[index0] += r * (w0 * W_inv);
					vertexPosition[index1] -= r * (w1 * W_inv);
#endif
				}
#if USE_ATOMICS
				).ScheduleParallel(clothMeshToken.jobHandle);
#else
				).Schedule(clothMeshToken.jobHandle);
#endif

#if USE_ATOMICS
				var deltaJob = new ApplyDeltaJob
				{
					vertexPosition = vertexPosition,
					vertexPositionDeltaX = clothMesh.vertexPositionDeltaX,
					vertexPositionDeltaY = clothMesh.vertexPositionDeltaY,
					vertexPositionDeltaZ = clothMesh.vertexPositionDeltaZ,
					vertexPositionDeltaW = clothMesh.vertexPositionDeltaW,
				};

				clothMeshToken.jobHandle = deltaJob.Schedule(vertexPosition.Length, 64, clothMeshToken.jobHandle);
#endif
			}

			Dependency = JobHandle.CombineDependencies(Dependency, clothMeshToken.jobHandle);
		}
		).WithoutBurst().Run();

#else// if USE_CHUNK_ITERATION

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

			Dependency = JobHandle.CombineDependencies(Dependency, clothMeshToken.jobHandle);
		}

#endif
	}
}
