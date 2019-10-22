using System.Collections;
using System.Collections.Generic;
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
        struct BuildSpacePartitionJob : IJobForEach<CarID, CarState>
        {
            public HighwaySpacePartition.ParallelWriter SpacePartition;

            public void Execute([ReadOnly] ref CarID id, [ReadOnly] ref CarState state)
            {
                SpacePartition.AddCar(id.Value, state.PositionOnTrack, state.Lane, state.FwdSpeed);
            }
        }

        struct GetNearestCarsJob : IJobForEach<ProximityData, CarState, CarSettings>
        {
            public HighwaySpacePartition SpacePartition;
            public float CarSize;

            public void Execute(
                ref ProximityData proximity,
                [ReadOnly] ref CarState state,
                [ReadOnly] ref CarSettings settings)
            {
                float maxDistance = math.max(settings.MergeSpace, settings.LeftMergeDistance) + CarSize;
                proximity.data = SpacePartition.GetNearestCars(
                    state.PositionOnTrack, state.Lane, maxDistance, maxDistance);
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
                Highway.instance.length(0),
                Game.instance.bucketDistance,
                Game.instance.maxNumCars,
                Allocator.TempJob);

            var buildJob = new BuildSpacePartitionJob { SpacePartition = SpacePartition.AsParallelWriter() };
            var buildDeps = buildJob.Schedule(this, inputDeps);

            var queryJob = new GetNearestCarsJob
            {
                SpacePartition = SpacePartition,
                CarSize = Game.instance.distanceToBack + Game.instance.distanceToFront
            };
            var queryDeps = queryJob.Schedule(this, buildDeps);

            return queryDeps;
        }
    }
}
