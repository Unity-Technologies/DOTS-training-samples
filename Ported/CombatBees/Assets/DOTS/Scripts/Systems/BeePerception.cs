using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class BeePerception : SystemBase
{
    private EntityQuery m_queryResources;

    protected override void OnCreate()
    {
        // Query list of Resources available to collect
        EntityQueryDesc queryResourcesDesc = new EntityQueryDesc()
        {
            All = new ComponentType[] { typeof(IsResource)},
            None = new ComponentType[] { typeof(IsCarried)}
        };
        m_queryResources = GetEntityQuery(queryResourcesDesc);
    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // TODO what about bees that are targetting Resources that have been
        // destroyed because they where returned to a hive?

        // Query for bees that are targetting Resources that are being carried by
        // another bee and clear their target Resource
        var cdfeForIsCarried = GetComponentDataFromEntity<IsCarried>(true);
        Entities
            .WithAll<IsBee>()
            .ForEach((Entity entity, in Target target) => {
                if (cdfeForIsCarried.HasComponent(target.Value))
                {
                    ecb.RemoveComponent<Target>(entity);
                }
        }).Schedule();
        
        var random = new Random(1234);

        // Get list of Resources available to collect
        var resources = m_queryResources.ToEntityArray(Allocator.TempJob);
        if (resources.Length >= 0)
        {
            // Query for bees that are not dead, carrying, attacking or returning
            // and set a random Resource as the target the bee is trying to collect         
            Entities
                .WithAll<IsBee>()
                .WithNone<Target, IsDead>()
                .ForEach((Entity entity) => {
                    // pick a random resource and add target component to bee
                    ecb.SetComponent(entity, new Target
                    {
                        Value = resources[random.NextInt(0, resources.Length)]
                    });
                    // tag the bee as now gathering
                    ecb.AddComponent<IsGathering>(entity);
            }).Schedule();
        }

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
