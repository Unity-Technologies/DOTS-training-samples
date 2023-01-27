using Aspects;
using Authoring;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Utils;

using Unity.Jobs;

namespace Jobs
{
    [BurstCompile]
    public struct CarSpawningJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Systems.CarSpawning.Slot> AllSlots;

        [ReadOnly] public NativeArray<float> LaneLengths;
        [ReadOnly] public Random Random;
        [ReadOnly] public Config Config;

        public EntityCommandBuffer.ParallelWriter Writer;

        [BurstCompile]
        public void Execute(int carNumber)
        {
            var car = Writer.Instantiate(carNumber, Config.CarPrefab);
            Systems.CarSpawning.Slot slot = AllSlots[carNumber];
            var laneNumber = slot.Lane;

            var segmentNumber = TransformationUtils.GetSegmentIndexFromDistance(slot.Distance, LaneLengths[laneNumber],
                Config.SegmentLength * Config.TrackSize);
            Writer.SetComponent(carNumber, car, new Car()
            {
                Distance = slot.Distance,
                Length = 1.0f,
                Speed = Random.NextFloat(Config.SpeedRange.x, Config.SpeedRange.y),
                DesiredSpeed = Random.NextFloat(Config.SpeedRange.x, Config.SpeedRange.y),
                Acceleration = Random.NextFloat(Config.AccelerationRange.x, Config.AccelerationRange.y),
                TrackLength = 1.0f,
                LaneNumber = laneNumber,
                NewLaneNumber = -1,
                LaneChangeProgress = 1.5f,
                LaneChangeClearance = Config.LaneChangeClearance,
                Color = float4.zero,
                SegmentNumber = segmentNumber,
                Index = carNumber
            });
            Writer.SetSharedComponent(carNumber, car, new SegmentNumber
            {
                SegmentId = segmentNumber
            });
        }
    }
}
