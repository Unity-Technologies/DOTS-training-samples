using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace HighwayRacers
{
    [UpdateBefore(typeof(CarStateSystem))]
    public class GetNearestCarsSystem : JobComponentSystem
    {
        [BurstCompile]
        struct BuildSpacePartitionJob : IJobForEach<CarID, CarState>
        {
            public HighwaySpacePartition.ParallelWriter SpacePartition;
            [ReadOnly] public DotsHighway DotsHighway;

            public void Execute([ReadOnly] ref CarID id, [ReadOnly] ref CarState state)
            {
                // Use the middle of the track to minimize error
                var pos = DotsHighway.GetEquivalentDistance(
                    state.PositionOnTrack, state.Lane, DotsHighway.NumLanes * 0.5f);
                SpacePartition.AddCar(id.Value, pos, state.Lane, state.FwdSpeed);
            }
        }

        [BurstCompile]
        struct GetNearestCarsJob : IJobForEach<CarID, CarState, ProximityData, CarSettings>
        {
            [ReadOnly] public HighwaySpacePartition SpacePartition;
            [ReadOnly] public DotsHighway DotsHighway;
            public float CarSize;

            public void Execute(
                [ReadOnly] ref CarID id,
                [ReadOnly] ref CarState state,
                ref ProximityData proximity,
                [ReadOnly] ref CarSettings settings)
            {
                float maxDistance = math.max(settings.MergeSpace, settings.LeftMergeDistance);
                // Use the middle of the track to minimize error
                var pos = DotsHighway.GetEquivalentDistance(
                    state.PositionOnTrack, state.Lane, DotsHighway.NumLanes * 0.5f);
                proximity.data = SpacePartition.GetNearestCars(
                    id.Value, pos, state.Lane, maxDistance, CarSize);
            }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            SpacePartition = new HighwaySpacePartition();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            SpacePartition.Dispose();
        }

        HighwaySpacePartition SpacePartition = new HighwaySpacePartition();

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            SpacePartition.Create(
                Highway.instance.DotsHighway.LaneLength(Highway.instance.DotsHighway.NumLanes * 0.5f),
                Game.instance.bucketDistance,
                Highway.instance.NumCars,
                Allocator.TempJob);

            var buildJob = new BuildSpacePartitionJob
            {
                SpacePartition = SpacePartition.AsParallelWriter(),
                DotsHighway = Highway.instance.DotsHighway
            };
            var buildDeps = buildJob.Schedule(this, inputDeps);

            var queryJob = new GetNearestCarsJob
            {
                SpacePartition = SpacePartition,
                DotsHighway = Highway.instance.DotsHighway,
                CarSize = Game.instance.distanceToBack + Game.instance.distanceToFront
            };
            return queryJob.Schedule(this, buildDeps);
        }
    }
}
