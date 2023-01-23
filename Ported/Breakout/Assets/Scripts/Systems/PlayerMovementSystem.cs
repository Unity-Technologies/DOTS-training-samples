using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
	[BurstCompile]
	public partial struct PlayerMovementSystem : ISystem
	{
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			
		}

		[BurstCompile]
		public void OnDestroy(ref SystemState state)
		{
			
		}
		
		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			var direction = new float3();
			direction.x = Input.GetAxis("Horizontal");
			direction.z = Input.GetAxis("Vertical");
			direction *= 5 * SystemAPI.Time.DeltaTime;
			
			foreach (var transform in SystemAPI.Query<TransformAspect>().WithAll<Player>())
			{
				transform.LocalPosition += direction;
			}
			
		}
	}
}