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
        var ecb = EntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        var arena = GetSingletonEntity<IsArena>();
        var arenaAABB = EntityManager.GetComponentData<Bounds>(arena).Value;

        var yellowBase = GetSingletonEntity<YellowBase>();
        var yellowBaseAABB = EntityManager.GetComponentData<Bounds>(yellowBase).Value;

        var blueBase = GetSingletonEntity<BlueBase>();
        var blueBaseAABB = EntityManager.GetComponentData<Bounds>(blueBase).Value;
        
        var random = Utils.GetRandom();

        Entities
             .WithName("GatherResource")
             .WithAll<IsGathering>()
             .ForEach((int entityInQueryIndex, Entity entity, ref TargetPosition targetPosition, in Target target, in Translation translation, in Team team) =>
             {
                 var updateTarget = false;
                 var targetAABB = arenaAABB;

                 var targetResource = target.Value;
                 
                 if (!HasComponent<IsCarried>(targetResource))
                 {
                     var targetTranslation = GetComponent<Translation>(targetResource);
                     
                     if (math.distancesq(translation.Value, targetTranslation.Value) < 0.025)
                     {
                         ecb.RemoveComponent<IsGathering>(entityInQueryIndex, entity);
                         ecb.AddComponent<IsReturning>(entityInQueryIndex, entity);
                         
                         ecb.RemoveComponent<OnCollision>(entityInQueryIndex, targetResource);
                         ecb.AddComponent<IsCarried>(entityInQueryIndex, targetResource);
                         
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

        EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
