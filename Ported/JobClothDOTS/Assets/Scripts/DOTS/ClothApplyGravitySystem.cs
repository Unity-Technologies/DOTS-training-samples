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
		public NativeArray<float3> vertexPosition;
		public float deltaTime;

		public void Execute(int i)
		{
			vertexPosition[i] += ClothConstants.gravity * deltaTime * deltaTime;
		}
	}

    protected override void OnUpdate()
    {
		Entities.ForEach((ref ClothMeshToken clothMeshToken, in ClothMesh clothMesh) =>
		{
			var job = new ApplyGravityJob
			{
				deltaTime = Time.DeltaTime,
				vertexPosition = clothMesh.vertexPosition,
			};

			clothMeshToken.jobHandle = job.Schedule(job.vertexPosition.Length, 64, clothMeshToken.jobHandle);
		}
		).WithoutBurst().Run();
	}
}
