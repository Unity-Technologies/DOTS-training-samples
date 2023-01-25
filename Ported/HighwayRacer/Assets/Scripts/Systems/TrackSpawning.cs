using Authoring;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Extensions;

namespace Systems
{
	public partial struct TrackSpawning : ISystem
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

            float segmentLength = 60.0f * config.TrackSize;
            const float curveRadius = 31.46f;
            var baseStraightTranslation = new float3(-segmentLength * 0.5f - curveRadius, 0.0f, 0.0f);
            var baseCurveTranslation = new float3(-segmentLength * 0.5f - curveRadius, 0.0f, segmentLength * 0.5f);

            const int numSegments = 4;
            for(var segIdx = 0; segIdx < numSegments; segIdx++)
            {
                var rotation = quaternion.RotateY(math.PI * 0.5f * segIdx);

                // Straights
                {
                    var translation = math.rotate(rotation, baseStraightTranslation);

                    var prefab = config.StraightTrackPrefab;
                    var track = state.EntityManager.Instantiate(prefab);
                    state.EntityManager.SetComponentData(track, new Track
                    {
                        Length = segmentLength,
                        SegmentId = segIdx * 2
                    });
                    state.EntityManager.SetComponentData(track, new LocalTransform
                    {
                        _Position = translation,
                        _Rotation = rotation,
                        _Scale = 1
                    });
                    state.EntityManager.AddComponentData(track, new PostTransformScale
                    {
                        Value = float3x3.Scale(1, 1, config.TrackSize)
                    });
                }

                // Curves
                {
                    var translation = math.rotate(rotation, baseCurveTranslation);

                    var prefab = config.CurvedTrackPrefab;

                    var track = state.EntityManager.Instantiate(prefab);
                    state.EntityManager.SetComponentData(track, new Track
                    {
                        Length = segmentLength,
                        SegmentId = segIdx * 2 + 1
                    });
                    state.EntityManager.SetComponentData(track, new LocalTransform
                    {
                        _Position = translation,
                        _Rotation = rotation,
                        _Scale = 1
                    });
                }
            }

            state.Enabled = false;
		}
	}
}
