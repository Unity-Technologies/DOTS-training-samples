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
            var builder = new BlobBuilder(Allocator.Temp);
            ref var root = ref builder.ConstructRoot<SplineHandle>();

            var size = 3;
            var segmentArray = builder.Allocate(ref root.Segments, size);

            for (int e = 0; e < size; e++)
            {
                Entity segment = ecb.CreateEntity();
                ecb.AddComponent(segment, new Segment
                {
                    Start = new float3(e, 0, 0),
                    End = new float3(e + 1, 0, 0),
                });

                segmentArray[e] = segment;
            }

            Entity splineEntity = ecb.CreateEntity();
            ecb.AddComponent(splineEntity, new Spline
            {
                Value = builder.CreateBlobAssetReference<SplineHandle>(Allocator.Persistent)
            });

            ecb.DestroyEntity(entity);
            builder.Dispose();
        }).Run();

        ecb.Playback(EntityManager);
    }
}
