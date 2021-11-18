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

        var ecb = ecbs.CreateCommandBuffer();

        Entities
            .WithAll<BeeCarryFoodMode>()
            .WithNone<Ballistic, Decay>()
            .WithReadOnly(beeDefinitions)
            .WithNativeDisableContainerSafetyRestriction(beeDefinitions)
            .ForEach((Entity entity, ref Bee myself, in Translation position, in TeamID team) =>
                {
                    var teamDef = beeDefinitions[team.Value];

                    var otherpos = GetComponent<Translation>(myself.TargetEntity);
                    if (math.distancesq(otherpos.Value, position.Value) < teamDef.attackRange)
                    {
                        ecb.RemoveComponent<BeeCarryFoodMode>(entity);
                        ecb.AddComponent(entity, new BeeIdleMode());
                        myself.TargetEntity = Entity.Null;
                        myself.CarriedFoodEntity = Entity.Null;
                    }
                }
            ).Schedule();
        
        ecbs.AddJobHandleForProducer(Dependency);
    }
}
