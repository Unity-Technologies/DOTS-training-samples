using Authoring;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Extensions;

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
            float TrackLength = 240.0f;
            float SegmentLength = 60.0f;
            // float TrackLength = config.TrackLength;
            // float SegmentLength = config.SegmentLength;

            // int segments = (int)math.ceil(TrackLength / SegmentLength);
            int segments = 8;
            int quarter = segments / 4;

			float3 position = float3.zero;
			quaternion rotation = quaternion.identity;
			quaternion dir = quaternion.Euler(math.forward());
            for(int i = 0; i < segments; i++)
            {
                var prefab = config.StraightTrackPrefab;
                if(i % 2 == 1)//1 to start with a straight
                {
                    //a turn on every quarter
                    prefab = config.CurvedTrackPrefab;
				    position += dir.ComputeAngles() * (SegmentLength / 2);
                    rotation = math.mul(rotation, quaternion.RotateY(math.PI / 2));

                    var track = state.EntityManager.Instantiate(prefab);
                    state.EntityManager.SetComponentData(track,  new Track
                    {
                        Length = SegmentLength,
                        SegmentId = i
                    });
                    state.EntityManager.SetComponentData(track, new LocalTransform
                    {
                        _Position = position,
                        _Rotation = math.mul(rotation, quaternion.RotateY(-math.PI / 2)),
                        _Scale = 1
                    });
                }
                else
                {
                    //straight pieces
				    position += dir.ComputeAngles() * SegmentLength;

                    var track = state.EntityManager.Instantiate(prefab);
                    state.EntityManager.SetComponentData(track,  new Track
                    {
                        Length = SegmentLength,
                        SegmentId = i
                    });
                    state.EntityManager.SetComponentData(track, new LocalTransform
                    {
                        _Position = position,
                        _Rotation = rotation,
                        _Scale = 1
                    });

                }
            }

			state.Enabled = false;
		}
	}
}
