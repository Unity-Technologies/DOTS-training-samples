using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class ClothApplyGravitySystem : SystemBase
{
	[BurstCompile]
	struct ApplyGravityJob : IJobParallelFor
	{
		[NoAlias] public NativeArray<float3> vertexPosition;
		[NoAlias] public NativeArray<float3> oldVertexPosition;
		[NoAlias, ReadOnly] public NativeArray<float> vertexInvMass;
		public float deltaTime;

		public void Execute(int i)
		{
			if (vertexInvMass[i] > 0.0f)
			{
				float3 oldVert = oldVertexPosition[i];
				float3 vert = vertexPosition[i];

				float3 startPos = vert;
				oldVert -= ClothConfig.gravity * deltaTime * deltaTime;
				vert += (vert - oldVert);

				vertexPosition[i] = vert;
				oldVertexPosition[i] = startPos;
			}
		}
	}

    protected override void OnUpdate()
    {
		Entities.ForEach((ref ClothMeshToken clothMeshToken, in ClothMesh clothMesh) =>
		{
			var job = new ApplyGravityJob
			{
				vertexPosition = clothMesh.vertexPosition,
				oldVertexPosition = clothMesh.vertexPositionOld,
				vertexInvMass = clothMesh.vertexInvMass,
				deltaTime = Time.DeltaTime,
			};

			clothMeshToken.jobHandle = job.Schedule(job.vertexPosition.Length, 64, clothMeshToken.jobHandle);
		}
		).WithoutBurst().Run();
	}
}
