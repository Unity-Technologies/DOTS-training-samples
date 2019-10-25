using HighwayRacers;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Assertions;

namespace HighwayRacers
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public class PrepareDrawingSystem : JobComponentSystem
    {
        HighwaySpacePartition SpacePartition;
        EntityQuery m_PositionQuery;

        // This will be used by Drawing system
        public NativeArray<LocalToWorld> Positions;
        public NativeArray<ColorComponent> Colors;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_PositionQuery = GetEntityQuery(
                ComponentType.ReadOnly<LocalToWorld>(),
                ComponentType.ReadOnly<ColorComponent>());
            SpacePartition = new HighwaySpacePartition();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            SpacePartition.Dispose();
        }

        // Jobs to prepare the space partition for next frame
        [BurstCompile]
        struct BuildSpacePartitionJob : IJobForEach<CarState>
        {
            public HighwaySpacePartition.ParallelWriter SpacePartition;
            [ReadOnly] public DotsHighway DotsHighway;

            public void Execute(ref CarState state)
            {
                // Use the middle of the track to minimize error
                state.PositionOnCenterTrack = DotsHighway.GetEquivalentDistance(
                    state.PositionOnTrack, state.Lane, (DotsHighway.NumLanes - 1) * 0.5f);
                SpacePartition.AddCar(state.PositionOnCenterTrack, state.Lane, state.FwdSpeed);
            }
        }

        [BurstCompile]
        struct GetNearestCarsJob : IJobForEach<CarState, ProximityData, CarSettings>
        {
            [ReadOnly] public HighwaySpacePartition SpacePartition;
            public float CarSize;

            public void Execute(
                [ReadOnly] ref CarState state,
                ref ProximityData proximity,
                [ReadOnly] ref CarSettings settings)
            {
                float maxDistance = math.max(settings.MergeSpace, settings.LeftMergeDistance);
                proximity.data = SpacePartition.GetNearestCars(
                    state.PositionOnCenterTrack, state.Lane, maxDistance, CarSize);
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (!Highway.USE_HYBRID_RENDERER)
            {
                Positions = m_PositionQuery.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);
                Colors = m_PositionQuery.ToComponentDataArray<ColorComponent>(Allocator.TempJob);
            }

            // Launch the update space partition jobs for next frame
            SpacePartition.Create(
                Highway.instance.DotsHighway.LaneLength(
                    (Highway.instance.DotsHighway.NumLanes - 1) * 0.5f),
                Game.instance.bucketDistance,
                Highway.instance.NumCars,
                Allocator.Persistent);

            var buildJob = new BuildSpacePartitionJob
            {
                SpacePartition = SpacePartition.AsParallelWriter(),
                DotsHighway = Highway.instance.DotsHighway
            };
            var buildDeps = buildJob.Schedule(this, inputDeps);
            DotsHighway.RegisterReaderJob(buildDeps);

            var queryJob = new GetNearestCarsJob
            {
                SpacePartition = SpacePartition,
                CarSize = Game.instance.distanceToBack + Game.instance.distanceToFront
            };
            return queryJob.Schedule(this, buildDeps);
        }
    }

    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [UpdateAfter(typeof(PrepareDrawingSystem))]
    public class DrawingSystem : ComponentSystem
    {
        Matrix4x4[] matrices = new Matrix4x4[1023];
        Vector4[] colorProperties = new Vector4[1023];

        protected override void OnUpdate()
        {
            if (Highway.USE_HYBRID_RENDERER)
                return;

            var ps = World.GetOrCreateSystem<PrepareDrawingSystem>();
            var properties = new MaterialPropertyBlock();
            var mat = Game.instance.entityMaterial;
            var mesh = Game.instance.entityMesh;

            Assert.IsTrue(ps.Colors.Length == ps.Positions.Length);

            var numCars = ps.Positions.Length;
            var numBatches = numCars / 1023;

            for (int batch = 0; batch < numBatches; batch++)
            {
                NativeArray<Matrix4x4>.Copy(ps.Positions.Reinterpret<Matrix4x4>().GetSubArray(batch * 1023, 1023),
                    matrices, 1023);
                NativeArray<Vector4>.Copy(ps.Colors.Reinterpret<Vector4>().GetSubArray(batch * 1023, 1023),
                    colorProperties, 1023);
                properties.SetVectorArray("_Color", colorProperties);
                Graphics.DrawMeshInstanced(mesh, 0, mat, matrices, 1023, properties);
            }

            var rest = numCars % 1023;
            if (rest > 0)
            {
                NativeArray<Matrix4x4>.Copy(ps.Positions.Reinterpret<Matrix4x4>().GetSubArray(numBatches * 1023, rest),
                    matrices, rest);
                NativeArray<Vector4>.Copy(ps.Colors.Reinterpret<Vector4>().GetSubArray(numBatches * 1023, rest),
                    colorProperties, rest);
                properties.SetVectorArray("_Color", colorProperties);
                Graphics.DrawMeshInstanced(mesh, 0, mat, matrices, rest, properties);
            }

            ps.Positions.Dispose();
            ps.Colors.Dispose();
        }
    }
}
