using Unity.Burst;
using Unity.Entities;

namespace Systems
{
	[BurstCompile]
	public partial struct BallSpawner : ISystem
	{
		public void OnCreate(ref SystemState state)
		{
			
		}

		public void OnDestroy(ref SystemState state)
		{
			
		}

		public void OnUpdate(ref SystemState state)
		{
			state.Enabled = false;
		}
	}
}