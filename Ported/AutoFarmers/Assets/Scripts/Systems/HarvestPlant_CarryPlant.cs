using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace AutoFarmers
{
    [UpdateAfter(typeof(SimulationSystemGroup))]
    public class HarvesPlant_CarryPlant : SystemBase
    {
        private EntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
            GetEntityQuery(ComponentType.ReadWrite<CellTypeElement>());
        }
        
        protected override void OnUpdate()
        {
            EntityCommandBuffer ecb = _entityCommandBufferSystem.CreateCommandBuffer();

            Entity grid = GetSingletonEntity<Grid>();
            Grid gridComponent = EntityManager.GetComponentData<Grid>(grid);
            int2 gridSize = gridComponent.Size;
            
            ComponentDataFromEntity<CellPosition> cellPositionAccessor = GetComponentDataFromEntity<CellPosition>();
            DynamicBuffer<CellTypeElement> cellTypeBuffer = EntityManager.GetBuffer<CellTypeElement>(grid);
            DynamicBuffer<CellEntityElement> cellEntityBuffer = EntityManager.GetBuffer<CellEntityElement>(grid);
            ComponentDataFromEntity<Sowed> sowedAccessor = GetComponentDataFromEntity<Sowed>();

            Entities
                .ForEach((Entity entity, in TakePlantToStore target, in Translation thisPos) =>
                {
                    ecb.SetComponent(target.Value, new Translation { Value = thisPos.Value });
                }).Schedule();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
