using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class BuildSplineSystem : SystemBase
{
    protected override void OnCreate()
    {
    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        Entities.ForEach((Entity entity, in BuildSpline buildSpline) =>
        {
            Entity segmentCollection = ecb.CreateEntity();
            ecb.AddComponent(segmentCollection, new SegmentCollection
            {
                Value = buildSpline.SegmentCollection
            });

            int startOffset = 0;
            for(int i = 0; i < buildSpline.SplineCollection.Value.Segments.Length; ++i)
            {
                var currentSize = buildSpline.SplineCollection.Value.Segments[i];

                var builder = new BlobBuilder(Allocator.Temp);
                ref var root = ref builder.ConstructRoot<SplineHandle>();
                var segmentArray = builder.Allocate(ref root.Segments, currentSize);
                for (int j = 0; j < currentSize; ++j)
                {
                    segmentArray[j] = buildSpline.AllSplines.Value.Segments[j + startOffset];
                }

                var splineEntity = ecb.CreateEntity();
                ecb.AddComponent(splineEntity, new Spline
                {
                    Value = builder.CreateBlobAssetReference<SplineHandle>(Allocator.Persistent)
                });
                builder.Dispose();
                startOffset += buildSpline.SplineCollection.Value.Segments[i];
            }

#if _HARDCODED_SPLINE
            {
                var builder = new BlobBuilder(Allocator.Temp);
                ref var root = ref builder.ConstructRoot<SegmentHandle>();
                var segmentArray = builder.Allocate(ref root.Segments, 5);

                var scale = 5.0f;
                segmentArray[0] = new SegmentData
                {
                    // A
                    Start = new float3(-1, 0, 1) * scale,
                    End = new float3(0, 0, 0) * scale,
                };

                segmentArray[1] = new SegmentData
                {
                    //B
                    Start = new float3(-1, 0, -1) * scale,
                    End = new float3(0, 0, 0) * scale,
                };

                segmentArray[2] = new SegmentData
                {
                    //C
                    Start = new float3(0, 0, 0) * scale,
                    End = new float3(1, 0, 0) * scale,
                };

                segmentArray[3] = new SegmentData
                {
                    //D
                    Start = new float3(1, 0, 0) * scale,
                    End = new float3(2, 0, 1) * scale,
                };

                segmentArray[4] = new SegmentData
                {
                    //E
                    Start = new float3(1, 0, 0) * scale,
                    End = new float3(2, 0, -1) * scale,
                };

                Entity segmentCollection = ecb.CreateEntity();
                ecb.AddComponent(segmentCollection, new SegmentCollection
                {
                    Value = builder.CreateBlobAssetReference<SegmentHandle>(Allocator.Persistent)
                });
                builder.Dispose();
            }

            //Creating green spline
            {
                var builder = new BlobBuilder(Allocator.Temp);
                ref var root = ref builder.ConstructRoot<SplineHandle>();
                var size = 3;
                var segmentArray = builder.Allocate(ref root.Segments, size);
                segmentArray[0] = 0;
                segmentArray[1] = 2;
                segmentArray[2] = 3;

                Entity splineEntity = ecb.CreateEntity();

                ecb.AddComponent(splineEntity, new Spline
                {
                    Value = builder.CreateBlobAssetReference<SplineHandle>(Allocator.Persistent)
                });
                builder.Dispose();
            }

            //Creating red spline
            {
                var builder = new BlobBuilder(Allocator.Temp);
                ref var root = ref builder.ConstructRoot<SplineHandle>();
                var size = 3;
                var segmentArray = builder.Allocate(ref root.Segments, size);
                segmentArray[0] = 1;
                segmentArray[1] = 2;
                segmentArray[2] = 4;

                Entity splineEntity = ecb.CreateEntity();
                ecb.AddComponent(splineEntity, new Spline
                {
                    Value = builder.CreateBlobAssetReference<SplineHandle>(Allocator.Persistent)
                });
                builder.Dispose();
            }
#endif
            ecb.DestroyEntity(entity);
        }).Run();

        ecb.Playback(EntityManager);
    }
}
