using Authoring;
using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Utils;
using Random = Unity.Mathematics.Random;

namespace Systems
{
    [BurstCompile]
    public partial struct CarSpawning : ISystem
    {
        public struct Slot
        {
            public int Lane;
            public float Distance;
        }

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


            NativeArray<float> laneLengths = new NativeArray<float>(config.NumLanes, Allocator.TempJob);
            NativeArray<int> laneSlotCount = new NativeArray<int>(config.NumLanes, Allocator.Temp);
            int totalSlots = 0;
            for (int i = 0; i < config.NumLanes; i++)
            {
                // Each lane has different length.
                var laneRadius = LaneSpawning.GetLaneRadius(i, config.NumLanes, Config.LaneOffset, Config.CurveRadius);
                var laneLength = LaneSpawning.GetLaneLength(laneRadius, config.TrackSize);
                laneLengths[i] = laneLength;
                float slotSize = config.LaneChangeClearance;
                int slotCount = (int)math.floor(laneLength / slotSize);
                totalSlots += slotCount;
                laneSlotCount[i] = slotCount;
            }
            NativeArray<Slot> allSlots = new NativeArray<Slot>(totalSlots, Allocator.TempJob);

            int count = 0;
            for (int i = 0; i < config.NumLanes; i++)
            {
                int slotCount = laneSlotCount[i];

                for (int j = 0; j < slotCount; j++)
                {
                    allSlots[count] = new Slot() { Lane = i, Distance = j * config.LaneChangeClearance };
                    count++;
                }
            }

            int n = allSlots.Length;
            while (n > 1)
            {
                int k = random.NextInt(n--);
                Slot temp = allSlots[n];
                allSlots[n] = allSlots[k];
                allSlots[k] = temp;
            }



            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
            var writer = ecb.AsParallelWriter();

            Jobs.CarSpawningJob testJob = new Jobs.CarSpawningJob
            {
                AllSlots = allSlots,
                LaneLengths = laneLengths,
                Random = random,
                Config = config,
                Writer = writer
            };

            int batchSize = 100;
            var jobHandle = testJob.Schedule(config.NumCars, batchSize);
            state.Dependency = jobHandle;
            jobHandle.Complete();
            ecb.Playback(state.EntityManager);
            ecb.Dispose();

            state.Enabled = false;
        }
    }
}
