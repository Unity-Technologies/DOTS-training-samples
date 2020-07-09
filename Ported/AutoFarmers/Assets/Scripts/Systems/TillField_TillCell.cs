using Unity.Entities;
using Unity.Mathematics;

namespace AutoFarmers
{
    [UpdateAfter(typeof(SimulationSystemGroup))]
    public class TillField_TillCell : SystemBase
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
            
            Entities
                .WithAll<TillField_Intent>()
                .WithAll<TargetReached>()
                .ForEach((Entity entity, in Target target) =>
                {
                    CellPosition cp = cellPositionAccessor[target.Value];
                    int index = (int) (cp.Value.x * gridSize.x + cp.Value.y);
                    
                    cellTypeBuffer[index] = new CellTypeElement() { Value = CellType.Tilled };
                    ecb.AddComponent<Tilled>(cellEntityBuffer[index].Value);

                    ecb.AddComponent<Cooldown>(entity, new Cooldown { Value = 0.1f });
                    ecb.RemoveComponent<Target>(entity);
                    ecb.RemoveComponent<TargetReached>(entity);
                }).Schedule();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
