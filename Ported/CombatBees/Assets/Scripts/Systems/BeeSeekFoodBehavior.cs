using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class BeeSeekFoodBehavior : SystemBase
{
    EndSimulationEntityCommandBufferSystem ecbs;

    protected override void OnCreate()
    {
        ecbs = World
            .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var globalDataEntity = GetSingletonEntity<GlobalData>();
        var globalData = GetComponent<GlobalData>(globalDataEntity);
        var beeDefinitions = GetBuffer<TeamDefinition>(globalDataEntity);

        var frameCount = UnityEngine.Time.frameCount +1;
        
        var dt = Time.DeltaTime;
        
        var ecb = ecbs.CreateCommandBuffer().AsParallelWriter();

        Entities
            .WithAll<BeeSeekFoodMode>()
            .WithNone<Ballistic, Decay>()
            .WithReadOnly(beeDefinitions)
            .WithNativeDisableContainerSafetyRestriction(beeDefinitions)
            .ForEach((Entity entity, int entityInQueryIndex, ref Bee myself, in Translation position, in TeamID team) =>
                {
                    var teamDef = beeDefinitions[team.Value];
                    var targeted = GetComponent<TargetedBy>(myself.TargetEntity);

                    if (targeted.Value != entity)
                    {
                        myself.TargetEntity = Entity.Null;
                        ecb.RemoveComponent<BeeSeekFoodMode>(entityInQueryIndex, entity);
                        ecb.AddComponent(entityInQueryIndex, entity, new BeeIdleMode());
                        return;
                    }

                    var otherpos = GetComponent<Translation>(myself.TargetEntity);
                    if (math.distancesq(otherpos.Value, position.Value) < teamDef.pickupFoodRange)
                    {
                        var random = Random.CreateFromIndex((uint)(entityInQueryIndex + frameCount));

                        // If the food is falling
                        ecb.RemoveComponent<Ballistic>(entityInQueryIndex, myself.TargetEntity);
                        ecb.RemoveComponent<BeeSeekFoodMode>(entityInQueryIndex, entity);
                        ecb.AddComponent(entityInQueryIndex, entity, new BeeCarryFoodMode());
                        ecb.AddComponent(entityInQueryIndex, myself.TargetEntity, new IsCarried());
                        myself.CarriedFoodEntity = myself.TargetEntity;
                        myself.TargetEntity = teamDef.hive;
                        myself.TargetOffset = new float3(
                            random.NextFloat(-4, 4),
                            random.NextFloat(-8, 8),
                            random.NextFloat(-8, 8)
                        );
                    }
                }
            ).ScheduleParallel();

        ecbs.AddJobHandleForProducer(Dependency);
    }
}
