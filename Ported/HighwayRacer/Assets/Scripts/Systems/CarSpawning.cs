using Authoring;
using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using Utils;
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


            int totalSlots = 0;
            NativeArray<NativeArray<float>> slots = new NativeArray<NativeArray<float>>(config.NumLanes, Allocator.Temp);
            NativeArray<int> slotIndex = new NativeArray<int>(config.NumLanes, Allocator.Temp);
            NativeArray<float> laneLengths = new NativeArray<float>(config.NumLanes, Allocator.Temp);
            for (int i = 0; i < config.NumLanes; i++)
            {
                // Each lane has different length.
                var laneRadius = LaneSpawning.GetLaneRadius(i, config.NumLanes, Config.LaneOffset, Config.CurveRadius);
                var laneLength = LaneSpawning.GetLaneLength(laneRadius, config.TrackSize);
                laneLengths[i] = laneLength;

                float slotSize = config.LaneChangeClearance;
                int slotCount = (int)math.floor(laneLength / slotSize);
                NativeArray<float> current = new NativeArray<float>(slotCount, Allocator.Temp);
                for (int j = 0; j < slotCount; j++)
                {
                    current[j] = j * slotSize;
                }

                int n = current.Length;
                while (n > 1)
                {
                    int k = random.NextInt(n--);
                    float temp = current[n];
                    current[n] = current[k];
                    current[k] = temp;
                }

                slots[i] = current;

                totalSlots += slotCount;
                slotIndex[i] = 0;
            }

            UnityEngine.Debug.Log($"Total Slots: {totalSlots}, Num Cars: {config.NumCars}");
            for (int i = 0; i < config.NumCars; i++)
            {
                var car = state.EntityManager.Instantiate(config.CarPrefab);

                var laneNumber = random.NextInt(config.NumLanes);
                int index = slotIndex[laneNumber]++;

                if (index > slots[laneNumber].Length - 1)
                    continue;

                var segmentNumber = TransformationUtils.GetSegmentIndexFromDistance(slots[laneNumber][index], laneLengths[laneNumber],
                    Config.SegmentLength * config.TrackSize);
                state.EntityManager.SetComponentData(car, new Car()
                {
                    Distance = slots[laneNumber][index],
                    Length = 1.0f,
                    Speed = random.NextFloat(config.SpeedRange.x, config.SpeedRange.y),
                    DesiredSpeed = random.NextFloat(config.SpeedRange.x, config.SpeedRange.y),
                    Acceleration = random.NextFloat(config.AccelerationRange.x, config.AccelerationRange.y),
                    TrackLength = 1.0f,
                    LaneNumber = laneNumber,
                    NewLaneNumber = -1,
                    LaneChangeProgress = 1.5f,
                    LaneChangeClearance = config.LaneChangeClearance,
                    Color = float4.zero,
                    SegmentNumber = segmentNumber,
                    Index = i
                });
                state.EntityManager.SetSharedComponent(car, new SegmentNumber
                {
                    SegmentId = segmentNumber
                });
            }

            state.Enabled = false;
        }
    }
}
