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
            //Fake Segment section
            {
                var builder = new BlobBuilder(Allocator.Temp);
                ref var root = ref builder.ConstructRoot<SegmentHandle>();
                var segmentArray = builder.Allocate(ref root.Segments, 5);

                segmentArray[0] = new SegmentData
                {
                    // A
                    Start = new float3(-1, 1, 0),
                    End = new float3(0, 0, 0),
                };

                segmentArray[1] = new SegmentData
                {
                    //B
                    Start = new float3(-1, -1, 0),
                    End = new float3(0, 0, 0),
                };

                segmentArray[2] = new SegmentData
                {
                    //C
                    Start = new float3(0, 0, 0),
                    End = new float3(1, 0, 0),
                };

                segmentArray[3] = new SegmentData
                {
                    //D
                    Start = new float3(1, 0, 0),
                    End = new float3(2, 1, 0),
                };

                segmentArray[4] = new SegmentData
                {
                    //E
                    Start = new float3(1, 0, 0),
                    End = new float3(2, -1, 0),
                };

                Entity segmentCollection = ecb.CreateEntity();
                //EntityManager.SetName(splineEntity, "Spline Red");
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
                //EntityManager.SetName(splineEntity, "Spline Red");
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
                //EntityManager.SetName(splineEntity, "Spline Green");
                ecb.AddComponent(splineEntity, new Spline
                {
                    Value = builder.CreateBlobAssetReference<SplineHandle>(Allocator.Persistent)
                });
                builder.Dispose();
            }

            ecb.DestroyEntity(entity);
        }).Run();

        ecb.Playback(EntityManager);
    }
}
