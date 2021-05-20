using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(BeeUpdateGroup))]
public class BeeIdleSystem : SystemBase
{
    private EntityQuery QueryResources;

    protected override void OnCreate()
    {
        // Query list of Resources available to collect
        EntityQueryDesc queryResourcesDesc = new EntityQueryDesc
        {
            All = new ComponentType[] { typeof(IsBee) },
            None = new ComponentType[] { typeof(IsAttacking), typeof(IsGathering), typeof(IsReturning), typeof(IsDead) }
        };

        QueryResources = GetEntityQuery(queryResourcesDesc);
    }

    protected override void OnUpdate()
    {
        var arena = GetSingletonEntity<IsArena>();
        var arenaAABB = EntityManager.GetComponentData<Bounds>(arena).Value;

        var random = Utils.GetRandom();

        Entities
            .WithStoreEntityQueryInField(ref QueryResources)
            .ForEach((ref TargetPosition targetPosition, in Translation translation) =>
            {
                if (math.distancesq(targetPosition.Value, translation.Value) < 0.025)
                {
                    targetPosition.Value = Utils.BoundedRandomPosition(arenaAABB, ref random);
                }
            }).Schedule();
    }
}
