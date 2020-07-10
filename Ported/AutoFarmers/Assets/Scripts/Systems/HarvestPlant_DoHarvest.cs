using Unity.Entities;
using Unity.Mathematics;

namespace AutoFarmers
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class HarvesPlant_DoHarvest : SystemBase
    {
        private EntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetExistingSystem<BeginSimulationEntityCommandBufferSystem>();
            GetEntityQuery(ComponentType.ReadWrite<CellTypeElement>());
        }
        
        protected override void OnUpdate()
        {
            EntityCommandBuffer ecb = _entityCommandBufferSystem.CreateCommandBuffer();

            Entity grid = GetSingletonEntity<Grid>();
            Grid gridComponent = EntityManager.GetComponentData<Grid>(grid);
            
            ComponentDataFromEntity<CellPosition> cellPositionAccessor = GetComponentDataFromEntity<CellPosition>();
            DynamicBuffer<CellTypeElement> cellTypeBuffer = EntityManager.GetBuffer<CellTypeElement>(grid);
            DynamicBuffer<CellEntityElement> cellEntityBuffer = EntityManager.GetBuffer<CellEntityElement>(grid);
            ComponentDataFromEntity<Sowed> sowedAccessor = GetComponentDataFromEntity<Sowed>();

            Entities
                .WithAll<HarvestPlant_Intent>()
                .WithAll<TargetReached>()
                .WithAll<HarvestPlant>()
                .ForEach((Entity entity, ref PathFindingTarget target, in Home home) =>
                {
                    Entity plant = target.Value;
                    CellPosition cp = cellPositionAccessor[target.Value];
                    int index = gridComponent.GetIndexFromCoords(cp.Value);
                    
                    var cellEntity = cellEntityBuffer[index].Value;
                    ecb.RemoveComponent<Sowed>(cellEntity);
                    ecb.SetComponent(cellEntity, new Cell { Type = CellType.Tilled });

                    //ecb.AddComponent<Cooldown>(entity, new Cooldown { Value = 0.1f });
                    //ecb.SetComponent<PathFindingTarget>(new PathFindingTarget { Value = home.Value });
                    target = new PathFindingTarget { Value = home.Value };
                    ecb.RemoveComponent<Target>(entity);
                    ecb.RemoveComponent<TargetReached>(entity);
                    ecb.RemoveComponent<HarvestPlant>(entity);
                    ecb.AddComponent<TakePlantToStore>(entity, new TakePlantToStore { Value = plant } );
                }).Schedule();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
