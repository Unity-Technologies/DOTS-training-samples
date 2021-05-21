using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(BeeUpdateGroup))]
[UpdateAfter(typeof(BeePerception))]
public class BeeGatheringSystem : SystemBase
{
    private EntityCommandBufferSystem EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        EntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var random = Utils.GetRandom();
        var ecb = EntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        var cdfeForTranslation = GetComponentDataFromEntity<Translation>(true);
        var yellowBase = GetSingletonEntity<YellowBase>();
        var yellowBaseAABB = EntityManager.GetComponentData<Bounds>(yellowBase).Value;

        var blueBase = GetSingletonEntity<BlueBase>();
        var blueBaseAABB = EntityManager.GetComponentData<Bounds>(blueBase).Value;

        var arena = GetSingletonEntity<IsArena>();
        var arenaAABB = EntityManager.GetComponentData<Bounds>(arena).Value;

        Entities
             .WithName("GatherResource")
             .WithReadOnly(cdfeForTranslation)
             .WithAll<IsGathering>()
             .ForEach((int entityInQueryIndex, Entity entity, ref TargetPosition targetPosition, in Target target, in Translation translation, in Team team) =>
             {
                 var updateTarget = false;
                 var targetAABB = arenaAABB;
                 
                 if (!HasComponent<IsCarried>(target.Value))
                 {
                     if (math.distancesq(translation.Value, cdfeForTranslation[target.Value].Value) < 0.025)
                     {
                         ecb.RemoveComponent<IsGathering>(entityInQueryIndex, entity);
                         ecb.AddComponent<IsReturning>(entityInQueryIndex, entity);
                         ecb.AddComponent<IsCarried>(entityInQueryIndex, target.Value);
                         ecb.RemoveComponent<OnCollision>(entityInQueryIndex, target.Value);

                         updateTarget = true;
                         targetAABB = team.Id == 0 ? yellowBaseAABB : blueBaseAABB;
                     }
                 }
                 else
                 {
                     ecb.RemoveComponent<Target>(entityInQueryIndex, entity);
                     ecb.RemoveComponent<IsGathering>(entityInQueryIndex, entity);
                     updateTarget = true;
                 }

                 if (updateTarget)
                 {
                     var newRandomPosition = Utils.BoundedRandomPosition(targetAABB, ref random);
                     newRandomPosition.y = targetAABB.Center.y;
                     targetPosition.Value = newRandomPosition;
                 }
             }).ScheduleParallel();

        // Check if the resource that the bees are targeting 
        /*Entities
             .WithAll<IsGathering>()
             .ForEach((Entity entity, ref TargetPosition targetPosition, in Target target) =>
             {
                 if (!HasComponent<IsCarried>(target.Value))
                 {
                     ecb.RemoveComponent<Target>(entity);
                     ecb.RemoveComponent<IsGathering>(entity);

                     targetPosition.Value = Utils.BoundedRandomPosition(arenaAABB, ref random);
                 }
             }).Schedule();*/

        EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
