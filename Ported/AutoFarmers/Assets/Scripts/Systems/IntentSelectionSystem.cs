using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class IntentSelectionSystem : SystemBase
{
    private Random _random;
    private EntityCommandBufferSystem _entityCommandBufferSystem;

    protected override void OnCreate()
    {
        _random = new Random(0x1234567);
        _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        Random random = _random;
        
        //Q: Use 'new' each update or store reference?
        EntityCommandBuffer.Concurrent ecb = _entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
        //Dependency = JobHandle.CombineDependencies(Dependency, ballHandle);

        Entities
            .WithNone<HarvestPlant_Intent>()
            .WithNone<PlantSeeds_Intent>()
            .WithNone<SmashRock_Intent>()
            .WithNone<TillField_Intent>()
            .ForEach((int entityInQueryIndex, Entity entity, ref Position position, in Velocity velocity) =>
            {
                int selection = random.NextInt(0, 4);

                switch (selection)
                {
                    case 0:
                        ecb.AddComponent(entityInQueryIndex, entity, new HarvestPlant_Intent());
                        break;
                    case 1:
                        ecb.AddComponent(entityInQueryIndex, entity, new HarvestPlant_Intent());
                        break;
                    case 2:
                        ecb.AddComponent(entityInQueryIndex, entity, new HarvestPlant_Intent());
                        break;
                    case 3:
                        ecb.AddComponent(entityInQueryIndex, entity, new HarvestPlant_Intent());
                        break;
                }
            }).ScheduleParallel();

        _entityCommandBufferSystem.AddJobHandleForProducer(Dependency); 
    }
}