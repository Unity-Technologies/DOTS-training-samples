using Authoring;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace Systems
{
	[BurstCompile]
	public partial struct BallSpawning : ISystem
	{
		public void OnCreate(ref SystemState state)
		{
			state.RequireForUpdate<Config>();
		}

		public void OnDestroy(ref SystemState state)
		{
			
		}

		public void OnUpdate(ref SystemState state)
		{
			var config = SystemAPI.GetSingleton<Config>();
			var random = new Random(111);

			for (int i = 0; i < config.NumBalls; i++)
			{
				var ball = state.EntityManager.Instantiate(config.BallPrefab);
				state.EntityManager.SetComponentData(ball,  new LocalTransform()
				{
					_Position = new float3(random.NextFloat(config.Boundary.x), 1, random.NextFloat(config.Boundary.y)),
					_Rotation = quaternion.identity,
					_Scale = 1
				});
				state.EntityManager.SetComponentData(ball, new URPMaterialPropertyBaseColor { Value = new float4(random.NextFloat(1), random.NextFloat(1), random.NextFloat(1), 1) });
			}
			
			state.Enabled = false;
		}
	}
}