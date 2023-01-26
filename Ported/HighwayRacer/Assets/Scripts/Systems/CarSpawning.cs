using Authoring;
using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace Systems
{
    [BurstCompile]
    public partial struct CarSpawning : ISystem
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

            for (int i = 0; i < config.NumCars; i++)
            {
                // TODO: Avoid overlapping
                // TODO: Get actual distance in the track, and assign segment id accordingly
                /*
                public float Distance;
                public float Length;
                public float Speed;
                public float Acceleration;
                public float TrackLength;
                public int LaneNumber;
                public float LaneChangeClearance;
                public float4 Color;
                public int SegmentNumber;
                */


                // Each lane has different length.

                var laneNumber = random.NextInt(config.NumLanes);
                var lane0Radius = Config.CurveRadius - Config.LaneOffset * (config.NumLanes - 1) * 0.5f;
                var currentLaneRadius = lane0Radius + laneNumber * Config.LaneOffset;

                var car = state.EntityManager.Instantiate(config.CarPrefab);

                var laneTotalLength = 60.0f * config.TrackSize + 2.0f * math.PI * currentLaneRadius;
                var distance = random.NextFloat(laneTotalLength);

                state.EntityManager.SetComponentData(car, new Car()
                {
                    Distance = random.NextFloat(99.0f),
                    Length = 1.0f,
                    // Speed = 0.0f,
                    Speed = random.NextFloat(config.SpeedRange.x, config.SpeedRange.y),
                    MaxSpeed = random.NextFloat(config.SpeedRange.x, config.SpeedRange.y),
                    Acceleration = random.NextFloat(config.SpeedRange.x, config.SpeedRange.y),
                    TrackLength = 1.0f,
                    LaneNumber = random.NextInt(4),
                    NewLaneNumber = -1,
                    LaneChangeProgress = -1.0f,
                    LaneChangeClearance = 1.5f,
                    Color = float4.zero,
                    SegmentNumber = 0,
                    Index = i
                });

            }

            state.Enabled = false;
        }
    }
}
