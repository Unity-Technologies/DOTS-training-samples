using Authoring;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
	[BurstCompile]
	public partial struct ObstacleSpawning : ISystem
	{
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			state.RequireForUpdate<Config>();
		}

		[BurstCompile]
		public void OnDestroy(ref SystemState state)
		{
			
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			var config = SystemAPI.GetSingleton<Config>();
			var random = new Random(1234);

			for (int i = 0; i < config.NumObstacles; i++)
			{
				var obstacle = state.EntityManager.Instantiate(config.ObstaclePrefab);
				state.EntityManager.SetComponentData(obstacle,  new LocalTransform()
				{
					_Position = new float3(random.NextFloat(config.Boundary.x), 1, random.NextFloat(config.Boundary.y)),
					_Rotation = quaternion.identity,
					_Scale = 3
				});
			}

			state.Enabled = false;
		}
	}
}