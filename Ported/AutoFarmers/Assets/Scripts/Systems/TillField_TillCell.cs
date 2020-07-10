using Unity.Entities;
using Unity.Mathematics;

namespace AutoFarmers
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class TillField_TillCell : SystemBase
    {
        private EntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetExistingSystem<BeginSimulationEntityCommandBufferSystem>();
            GetEntityQuery(ComponentType.ReadWrite<CellTypeElement>());
            
            RequireSingletonForUpdate<GameSettings>();
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
                .ForEach((Entity entity, in PathFindingTarget target) =>
                {
                    CellPosition cp = cellPositionAccessor[target.Value];
                    int index = gridComponent.GetIndexFromCoords(cp.Value.x, cp.Value.y);

                    if (cellTypeBuffer[index].Value == CellType.Raw)
                    {
                        var cellEntity = cellEntityBuffer[index].Value;
                        ecb.SetComponent(cellEntity, new Cell { Type = CellType.Tilled });
                        ecb.AddComponent<Tilled>(cellEntity);
                    }
                    ecb.AddComponent<Cooldown>(entity, new Cooldown { Value = 0.1f });
                    ecb.RemoveComponent<PathFindingTarget>(entity);
                    ecb.RemoveComponent<Target>(entity);
                    ecb.RemoveComponent<TargetReached>(entity);
                }).Schedule();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
