using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;

using src.Components;

public class OmniWorkerSystem: SystemBase
{
    protected override void OnUpdate()
    {
        var time = Time.ElapsedTime;

        var updateWorkersWithoutBucket = Entities
            .WithAll<OmniWorkerTag>()
            .WithNone<WorkerIsHoldingBucket>()            
            .ForEach((ref Translation translation, ref Position position) =>
            {
                var query = GetEntityQuery(new EntityQueryDesc {
                    All = new ComponentType[] { typeof(src.Components.Bucket) }
                });
                try
                {
                    var chunks = query.CreateArchetypeChunkArray(Allocator.Persistent);
                    try
                    {
                        var bucketTypeHandle = GetComponentTypeHandle<src.Components.Bucket>(false);
                        for (int i = 0; i < chunks.Length; ++i)
                        {
                            var buckets = chunks[i].GetNativeArray(bucketTypeHandle);
                            // Get nearest empty bucket: O(n)
                            // If close enough to the bucket
                            //     pick it up
                            // else
                            //     move towards the nearest bucket
                        }
                    }
                    finally
                    {
                        chunks.Dispose();
                    }
                }
                finally
                {
                    query.Dispose();
                }
            }).ScheduleParallel(Dependency);

        var updateWorkerWithBucket = Entities
            .WithAll<OmniWorkerTag>()
            .WithAll<WorkerIsHoldingBucket>()
            .ForEach((ref Translation translation, ref Position position) =>
            {
                // Get bucket reference
                // If bucket is full
                if (true)
                {
                    //     Get nearest fire spot
                    //     If not close enough to the fire spot 
                    //         move towards the fire spot                   
                }
                else
                {
                    var query = GetEntityQuery(new EntityQueryDesc
                    {
                        All = new ComponentType[] { typeof(src.Components.WaterTag) }
                    });
                    try
                    {
                        var chunks = query.CreateArchetypeChunkArray(Allocator.Persistent);
                        try
                        {
                            var waterTypeHandle = GetComponentTypeHandle<src.Components.Bucket>(false);
                            for (int i = 0; i < chunks.Length; ++i)
                            {
                                var waterSources = chunks[i].GetNativeArray(waterTypeHandle);
                                // Get nearest water spot
                                // If not close enough to the spot 
                                //      move towards the water spot                
                            }
                        }
                        finally
                        {
                            chunks.Dispose();
                        }
                    }
                    finally
                    {
                        query.Dispose();
                    }
                }                             
            }).ScheduleParallel(Dependency);

        Dependency = updateWorkerWithBucket;
    }
}
