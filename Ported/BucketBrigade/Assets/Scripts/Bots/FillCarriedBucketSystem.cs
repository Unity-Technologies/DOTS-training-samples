using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[UpdateAfter(typeof(MoveTowardsTargetSystem))]
[UpdateBefore(typeof(StartNextCommandSystem))]
public class FillCarriedBucketSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;
    private EntityQuery bucketQuery;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
        
        bucketQuery = GetEntityQuery(ComponentType.ReadOnly<Volume>());
    }
    
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();
        
        NativeArray<Volume> bucketVolumes = bucketQuery.ToComponentDataArrayAsync<Volume>(Allocator.TempJob, out JobHandle j1);
        NativeArray<Entity> bucketEntities = bucketQuery.ToEntityArrayAsync(Allocator.TempJob, out JobHandle j2);
        Dependency = JobHandle.CombineDependencies(Dependency, j1, j2);
        
        Entities
            .WithName("FillBucket")
            .WithDisposeOnCompletion(bucketVolumes)
            .WithDisposeOnCompletion(bucketEntities)
            .ForEach((int entityInQueryIndex,
                ref HasBucket hasBucket, ref FillingBucket fillingBucket,
                in Target target) =>
            {
                if (hasBucket.PickedUp && target.ReachedTarget && fillingBucket.Filling)
                {
                    float oldVolume = GetVolume(hasBucket.Entity, bucketEntities, bucketVolumes);
                    float newVolume = math.min(oldVolume + FILL_RATE * deltaTime, MAX_FILL);
                    
                    ecb.SetComponent(entityInQueryIndex, hasBucket.Entity, new Volume { Value = newVolume });
                    
                    fillingBucket.Full = newVolume >= MAX_FILL;
                    if (fillingBucket.Full)
                        fillingBucket.Filling = false;
                }
            }).ScheduleParallel();
        
        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }

    static float GetVolume(in Entity bucketEntity, in NativeArray<Entity> bucketEntities, in NativeArray<Volume> bucketVolumes)
    {
        // Make these a dictionary?
        for (int i = 0; i < bucketEntities.Length; i++)
        {
            if (bucketEntity == bucketEntities[i])
                return bucketVolumes[i].Value;
        }

        return 0f;
    }

    private const float FILL_RATE = 1f;    // Get from config.
    private const float MAX_FILL = 1f;
}