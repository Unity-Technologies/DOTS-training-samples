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
            var track = InstantiateTrack(ref state, config, 100, 0);

            state.Enabled = false;
		}

        private Entity InstantiateTrack(ref SystemState state, Config config, float length, int segmentId)
        {
            var track = state.EntityManager.Instantiate(config.StraightTrackPrefab);
            var trackLength = 100;

            state.EntityManager.SetComponentData(track,  new Track
            {
                Length = trackLength,
                SegmentId = 0
            });
            state.EntityManager.SetComponentData(track, new LocalTransform
            {
                _Position = float3.zero,
                _Rotation = quaternion.identity,
                _Scale = 1
            });
            state.EntityManager.AddComponentData(track, new PostTransformScale
            {
                Value = float3x3.Scale(1, 1, trackLength)
            });
            return track;
        }
	}
}
