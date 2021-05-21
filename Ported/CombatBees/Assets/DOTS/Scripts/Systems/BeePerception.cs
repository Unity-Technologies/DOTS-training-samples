using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(BeeUpdateGroup))]
public class BeePerception : SystemBase
{
    private EntityQuery QueryResources;

    protected override void OnCreate()
    {
        // Query list of Resources available to collect
        var queryResourcesDesc = new EntityQueryDesc
        {
            All = new ComponentType[] { typeof(IsResource) },
            None = new ComponentType[] { typeof(IsCarried), typeof(HasGravity), typeof(LifeSpan) }
        };
        
        QueryResources = GetEntityQuery(queryResourcesDesc);
    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        
        var random = Utils.GetRandom();

        // TODO what about bees that are targeting Resources that have been
        // destroyed because they where returned to a hive?

        // Query for bees that are targeting Resources that are being carried by
        // another bee and clear their target Resource
        Entities
            .WithName("RemoveResourceTarget")
            .WithNone<IsReturning>()
            .WithAll<IsBee>()
            .ForEach((Entity entity, in Target target) =>
            {
                var targetEntity = target.Value;
                
                if (HasComponent<IsCarried>(targetEntity))
                {
                    ecb.RemoveComponent<Target>(entity);
                }
            }).Run();

        // Get list of Resources available to collect
        var resources = QueryResources.ToEntityArray(Allocator.TempJob);
        
        if (resources.Length > 0)
        {
            // Query for bees that are not dead, carrying, attacking or returning
            // and set a random Resource as the target the bee is trying to collect         
            Entities
                .WithName("TargetResource")
                .WithAll<IsBee>()
                .WithNone<Target, IsAttacking, IsDead>()
                .ForEach((Entity entity) =>
                {
                    // tag the bee as now gathering
                    ecb.AddComponent<IsGathering>(entity);
                    
                    // pick a random resource and add target component to bee
                    ecb.AddComponent(entity, new Target
                    {
                        Value = resources[random.NextInt(0, resources.Length)]
                    });
                }).Run();
        }

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
