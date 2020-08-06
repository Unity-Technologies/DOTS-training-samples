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
            //Build the System collection
            Entity segmentCollection = ecb.CreateEntity();
            ecb.AddComponent(segmentCollection, new SegmentCollection
            {
                Value = buildSpline.SegmentCollection
            });

            //Create all spline entities
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

                var spline = new Spline
                {
                    Value = builder.CreateBlobAssetReference<SplineHandle>(Allocator.Persistent)
                };

                var category = new SplineCategory
                {
                    category = buildSpline.SplineCategory.Value.Segments[i]
                };

                var splineEntity = ecb.CreateEntity();
                ecb.AddComponent(splineEntity, spline);
                ecb.AddComponent(splineEntity, category);
                builder.Dispose();
                startOffset += buildSpline.SplineCollection.Value.Segments[i];
            }

            ecb.DestroyEntity(entity);
        }).Run();

        ecb.Playback(EntityManager);
    }
}
