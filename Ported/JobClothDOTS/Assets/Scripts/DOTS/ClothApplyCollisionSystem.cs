using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(ClothApplyConstraintsSystem))]
public class ClothApplyCollisionSystem : SystemBase
{
	[BurstCompile]
	struct ApplyCollisionJob : IJobParallelFor
	{
		[NoAlias] public NativeArray<float3> vertexPosition;
		public float groundHeight;

		public void Execute(int i)
		{
			var position = vertexPosition[i];
			if (position.y < groundHeight)
				position.y = groundHeight;

			vertexPosition[i] = position;
		}
	}

    protected override void OnUpdate()
    {
		Entities.ForEach((ref ClothMeshToken clothMeshToken, in ClothMesh clothMesh) =>
		{
			var job = new ApplyCollisionJob
			{
				vertexPosition = clothMesh.vertexPosition,
				groundHeight = ClothConfig.groundHeight,
			};

			clothMeshToken.jobHandle = job.Schedule(job.vertexPosition.Length, 64, clothMeshToken.jobHandle);
		}
		).WithoutBurst().Run();
	}
}
