using System.Threading;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace HighwayRacer
{
    [UpdateAfter(typeof(AvoidanceMergeSpeedSys))]
    public class SetTransformAndColorSys : SystemBase
    {
        private EntityQuery query;

        private ArchetypeChunkComponentType<LocalToWorld> transformType;
        private ArchetypeChunkComponentType<URPMaterialPropertyBaseColor> colorType;

        private TransformAndColorJob[] jobArray = new TransformAndColorJob[100]; // bigger than we'll ever need

        protected override void OnCreate()
        {
            query = GetEntityQuery(typeof(LocalToWorld), typeof(URPMaterialPropertyBaseColor));
            base.OnCreate();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            query.Dispose();
        }

        public readonly static float3 cruiseColor = new float3(0.5f, 0.5f, 0.5f);
        public readonly static float3 fastestColor = new float3(0, 1.0f, 0);
        public readonly static float3 slowestColor = new float3(1.0f, 0.0f, 0);

        public static void setColor(in Car car, int entityIdx, ref NativeArray<URPMaterialPropertyBaseColor> colors)
        {
            URPMaterialPropertyBaseColor color;
            if (car.Speed >= car.DesiredSpeedUnblocked)
            {
                var percentage = (car.Speed - car.DesiredSpeedUnblocked) / (car.DesiredSpeedOvertake - car.DesiredSpeedUnblocked);
                color.Value = new float4(math.lerp(cruiseColor, fastestColor, percentage), 1.0f);
            }
            else
            {
                var percentage = (car.Speed - CarSpawnSys.minSpeed) / (car.DesiredSpeedUnblocked - CarSpawnSys.minSpeed);
                color.Value = new float4(math.lerp(slowestColor, cruiseColor, percentage), 1.0f);
            }

            colors[entityIdx] = color;
        }

        public static void setTransform(ref Car car, int entityIdx, ref NativeArray<LocalToWorld> transforms, ref RoadSegment seg)
        {
            var transform = seg.Transform;
            var laneOffsetDist = car.LaneOffsetDist();
            var translation = seg.Transform.c3.xyz;
            translation += seg.DirectionLaneOffset * laneOffsetDist;

            if (seg.IsCurved())
            {
                var percentage = car.Pos / seg.Length;
                var radians = math.radians(math.lerp(seg.RotDegrees, seg.RotDegrees + 90, percentage));
                transform = RoadSys.GetRotYMatrix(radians);
                
                // rotate the origin around pivot to get displacement
                var radius = seg.Radius + (laneOffsetDist * RoadSys.laneWidth);
                float3 pivot = new float3();
                float3 displacement = float3.zero;

                switch (seg.Direction)
                {
                    case Cardinal.UP:
                        pivot = new float3(radius, 0, 0);
                        break;
                    case Cardinal.DOWN:
                        pivot = new float3(-radius, 0, 0);
                        break;
                    case Cardinal.LEFT:
                        pivot = new float3(0, 0, radius);
                        break;
                    case Cardinal.RIGHT:
                        pivot = new float3(0, 0, -radius);
                        break;
                }

                // rotate displacement by angle around pivot
                var angle = math.lerp(math.radians(0), math.radians(-90), percentage); // -90 because cos & sin assume counter-clockwise
                displacement -= pivot;
                var c = math.cos(angle);
                var s = math.sin(angle);
                float x = displacement.x * c - displacement.z * s;
                displacement.z = displacement.z * c + displacement.x * s;
                displacement.x = x;
                displacement += pivot;

                translation += displacement;
            }
            else
            {
                translation += car.Pos * seg.DirectionVec;
            }
            
            transform.c3 = new float4(translation, 1);
            transforms[entityIdx] = new LocalToWorld() {Value = transform};
        }

        protected override void OnUpdate()
        {
            var chunks = query.CreateArchetypeChunkArray(Allocator.TempJob); // todo : could this be persistent and created only upon respawning cars?

            var nJobs = (chunks.Length >= JobsUtility.JobWorkerCount) ? JobsUtility.JobWorkerCount : chunks.Length;
            var nChunksPerJob = chunks.Length / nJobs;
            var remainder = chunks.Length % nJobs;

            // todo should be one job for each core

            var buckets = RoadSys.CarBuckets;
            var roadSegments = RoadSys.roadSegments;

            transformType = GetArchetypeChunkComponentType<LocalToWorld>();
            colorType = GetArchetypeChunkComponentType<URPMaterialPropertyBaseColor>();

            buckets.FirstNonEmptyBucket(out int bucketIdx);
            var firstCarIdx = 0;
            var firstChunkIdx = 0;
            for (int i = 0; i < nJobs; i++)
            {
                // todo profile cost of all this with the job execute commented out

                jobArray[i] = new TransformAndColorJob()
                {
                    chunks = chunks, // todo get query to get chunks

                    firstChunkIdx = firstChunkIdx,
                    lastChunkIdx = firstChunkIdx + (i < nJobs - 1 ? nChunksPerJob : nChunksPerJob + remainder) - 1,

                    firstBucketIdx = bucketIdx,
                    firstCarIdx = firstCarIdx,

                    carBuckets = buckets,
                    roadSegments = roadSegments,

                    transformType = transformType,
                    colorType = colorType,
                };

                if (i < nJobs - 1)
                {
                    // how many cars in these chunks
                    var nCars = 0;
                    for (int chunkIdx = firstChunkIdx; chunkIdx < (firstChunkIdx + nChunksPerJob); chunkIdx++)
                    {
                        nCars += chunks[chunkIdx].Count;
                    }

                    buckets.NextNthCar(ref bucketIdx, ref firstCarIdx, nCars);

                    firstChunkIdx += nChunksPerJob;
                }
            }

            var combined = new JobHandle();
            for (int i = 0; i < nJobs; i++)
            {
                combined = JobHandle.CombineDependencies(combined, jobArray[i].Schedule(Dependency));
            }

            combined.Complete();

            chunks.Dispose();
        }
    }

    [BurstCompile(CompileSynchronously = true)]
    public struct TransformAndColorJob : IJob
    {
        [NativeDisableContainerSafetyRestriction]
        public NativeArray<ArchetypeChunk> chunks;

        [NativeDisableContainerSafetyRestriction]
        public NativeArray<RoadSegment> roadSegments;

        public CarBuckets carBuckets;

        [NativeDisableContainerSafetyRestriction]
        public ArchetypeChunkComponentType<LocalToWorld> transformType;

        [NativeDisableContainerSafetyRestriction]
        public ArchetypeChunkComponentType<URPMaterialPropertyBaseColor> colorType;

        public int firstChunkIdx;
        public int lastChunkIdx;

        public int firstBucketIdx;
        public int firstCarIdx;

        public void Execute()
        {
            var bucketIdx = firstBucketIdx;
            var bucket = carBuckets.GetCars(bucketIdx);
            RoadSegment segment = roadSegments[bucketIdx];

            var carIdx = firstCarIdx;

            for (int chunkIdx = firstChunkIdx; chunkIdx <= lastChunkIdx; chunkIdx++)
            {
                var chunk = chunks[chunkIdx];

                var transforms = chunk.GetNativeArray(transformType);
                var colors = chunk.GetNativeArray(colorType);

                for (int entityIdx = 0; entityIdx < chunk.Count; entityIdx++, carIdx++)
                {
                    while (carIdx >= bucket.Length)
                    {
                        carIdx = 0;
                        bucketIdx++;
                        bucket = carBuckets.GetCars(bucketIdx);
                        segment = roadSegments[bucketIdx];
                    }

                    Car car = bucket[carIdx];

                    SetTransformAndColorSys.setTransform(ref car, entityIdx, ref transforms, ref segment);
                    SetTransformAndColorSys.setColor(in car, entityIdx, ref colors);
                }
            }
        }
    }
}