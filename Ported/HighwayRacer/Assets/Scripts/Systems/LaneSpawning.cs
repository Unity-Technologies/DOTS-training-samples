using Authoring;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace System
{
    [BurstCompile]
    public partial struct LaneSpawning : ISystem
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
            for (var laneNumber = 0; laneNumber < config.NumLanes; laneNumber++)
            {
                var lane = state.EntityManager.Instantiate(config.LanePrefab);
                var laneRadius = GetLaneRadius(laneNumber, config.NumLanes, Config.LaneOffset, Config.CurveRadius);
                var laneLength = GetLaneLength(laneNumber, config.TrackSize);
                state.EntityManager.SetComponentData(lane, new Lane()
                {

                    LaneNumber = laneNumber,
                    SegmentNumber = 0,
                    LaneLength = laneLength,
                    LaneRadius = laneRadius
                });
            }
            state.Enabled = false;
        }

        private static float GetLaneRadius(int laneNumber, int numLanes, float laneOffset, float curveRadius)
        {
            var lane0Radius = curveRadius - laneOffset * (numLanes - 1) * 0.5f;
            return  lane0Radius + laneNumber * laneOffset;
        }
        private static float GetLaneLength(float laneRadius, float trackScale)
        {
            return 4 * Config.SegmentLength * trackScale + 2.0f * math.PI * laneRadius;
        }

    }
    
}
