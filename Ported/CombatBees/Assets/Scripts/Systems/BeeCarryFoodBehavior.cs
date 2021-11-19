using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class BeeCarryFoodBehavior : SystemBase
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
        var beeDefinitions = GetBuffer<TeamDefinition>(globalDataEntity);

        var ecb = ecbs.CreateCommandBuffer().AsParallelWriter();

        Entities
            .WithAll<BeeCarryFoodMode>()
            .WithNone<Ballistic, Decay>()
            .WithReadOnly(beeDefinitions)
            .WithNativeDisableContainerSafetyRestriction(beeDefinitions)
            .ForEach((Entity entity, int entityInQueryIndex, ref Bee myself, in Translation position, in TeamID team) =>
                {
                    var teamDef = beeDefinitions[team.Value];

                    var otherpos = GetComponent<Translation>(myself.TargetEntity);
                    if (math.distancesq(otherpos.Value + myself.TargetOffset, position.Value) < teamDef.attackRange)
                    {
                        ecb.RemoveComponent<BeeCarryFoodMode>(entityInQueryIndex, entity);
                        ecb.AddComponent(entityInQueryIndex, entity, new BeeIdleMode());
                        myself.TargetEntity = Entity.Null;
                        myself.CarriedFoodEntity = Entity.Null;
                    }
                }
            ).ScheduleParallel();
        
        ecbs.AddJobHandleForProducer(Dependency);
    }
}
