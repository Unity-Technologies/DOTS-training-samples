using Authoring;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
	public partial struct TrackSpawner : ISystem
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
		
			// TODO: actual track authoring and placement. For now, just slap a single straight segment into the scene
			var track = state.EntityManager.Instantiate(config.StraightTrackPrefab);
			state.EntityManager.SetComponentData(track,  new Track
			{
				Length = 100,
				SegmentId = 0
			});
			state.EntityManager.SetComponentData(track, new LocalTransform
			{
				_Position = float3.zero,
				_Rotation = quaternion.identity,
				_Scale = 1
			});
			
			
			state.Enabled = false;
		}
	}
}