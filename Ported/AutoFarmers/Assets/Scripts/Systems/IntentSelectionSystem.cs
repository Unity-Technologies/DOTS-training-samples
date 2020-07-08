using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class IntentSelectionSystem : SystemBase
{
    private Random _random;
    //private EntityCommandBuffer _entityCommandBuffer;

    protected override void OnCreate()
    {
        _random = new Random(0x1234567);
        //_entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);
    }

    protected override void OnUpdate()
    {
        Random random = _random;
        
        //Q: Use 'new' each update or store reference?
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities
            .WithNone<HarvestPlant_Intent>()
            .WithNone<PlantSeeds_Intent>()
            .WithNone<SmashRock_Intent>()
            .WithNone<TillField_Intent>()
            .ForEach((Entity entity, ref Position position, in Velocity velocity) =>
            {
                int selection = random.NextInt(0, 4);

                switch (selection)
                {
                    case 0:
                        ecb.AddComponent(entity, new HarvestPlant_Intent());
                        break;
                    case 1:
                        ecb.AddComponent(entity, new HarvestPlant_Intent());
                        break;
                    case 2:
                        ecb.AddComponent(entity, new HarvestPlant_Intent());
                        break;
                    case 3:
                        ecb.AddComponent(entity, new HarvestPlant_Intent());
                        break;
                }
            }).ScheduleParallel();

        ecb.Playback(EntityManager);
    }
}