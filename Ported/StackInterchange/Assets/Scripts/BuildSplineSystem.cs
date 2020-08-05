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
            Entity segment_A = ecb.CreateEntity();
            //EntityManager.SetName(segment_A, "Segment A");
            ecb.AddComponent(segment_A, new Segment
            {
                Start = new float3(-1, -1, 0),
                End = new float3(0, 0, 0),
            });

            Entity segment_B = ecb.CreateEntity();
            //EntityManager.SetName(segment_B, "Segment B");
            ecb.AddComponent(segment_B, new Segment
            {
                Start = new float3(-1, 1, 0),
                End = new float3(0, 0, 0),
            });

            Entity segment_C = ecb.CreateEntity();
            //EntityManager.SetName(segment_C, "Segment C");
            ecb.AddComponent(segment_C, new Segment
            {
                Start = new float3(0, 0, 0),
                End = new float3(1, 0, 0),
            });

            Entity segment_D = ecb.CreateEntity();
            //EntityManager.SetName(segment_D, "Segment D");
            ecb.AddComponent(segment_D, new Segment
            {
                Start = new float3(1, 0, 0),
                End = new float3(2, 1, 0),
            });

            Entity segment_E = ecb.CreateEntity();
            //EntityManager.SetName(segment_D, "Segment E");
            ecb.AddComponent(segment_E, new Segment
            {
                Start = new float3(1, 0, 0),
                End = new float3(2, -1, 0),
            });

            //Creating green spline
            {
                var builder = new BlobBuilder(Allocator.Temp);
                ref var root = ref builder.ConstructRoot<SplineHandle>();
                var size = 3;
                var segmentArray = builder.Allocate(ref root.Segments, size);
                segmentArray[0] = segment_A;
                segmentArray[1] = segment_C;
                segmentArray[2] = segment_D;

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
                segmentArray[0] = segment_B;
                segmentArray[1] = segment_C;
                segmentArray[2] = segment_E;

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
