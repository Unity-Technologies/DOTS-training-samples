using Unity.Entities;
using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class IntentSelectionSystem : SystemBase
{
    private EntityCommandBufferSystem _entityCommandBufferSystem;

    protected override void OnCreate()
    {
        _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer.Concurrent ecb = _entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

        Entities
            .WithNone<HarvestPlant_Intent>()
            .WithNone<PlantSeeds_Intent>()
            .WithNone<SmashRock_Intent>()
            .WithNone<TillField_Intent>()
            .ForEach((int entityInQueryIndex, Entity entity, ref RandomSeed randomSeed, in Farmer_Tag tag) =>
            {
                Random random = new Random(randomSeed.Value);
                int selection = random.NextInt(0, 3);
                randomSeed.Value = random.state;

                switch (selection)
                {
                    case 0:
                        ecb.AddComponent(entityInQueryIndex, entity, new HarvestPlant_Intent());
                        break;
                    case 1:
                        ecb.AddComponent(entityInQueryIndex, entity, new PlantSeeds_Intent());
                        break;
                    case 2:
                        ecb.AddComponent(entityInQueryIndex, entity, new TillField_Intent());
                        break;
                  /*  case 3:
                        ecb.AddComponent(entityInQueryIndex, entity, new SmashRock_Intent());
                        break;*/

                }

               // ecb.RemoveComponent<Target>(entityInQueryIndex, entity);
               // ecb.RemoveComponent<TargetReached>(entityInQueryIndex, entity);

            }).ScheduleParallel();

        _entityCommandBufferSystem.AddJobHandleForProducer(Dependency); 
    }
}