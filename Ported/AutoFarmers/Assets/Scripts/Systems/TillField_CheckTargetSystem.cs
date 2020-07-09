using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace AutoFarmers
{
    public class TillField_CheckTargetSystem : SystemBase
    {
        private EntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

            //NOTE: Workaround for DynamicBuffer Not Readonly bug
            //      Using GetSingleton then GetBuffer doesn't use query building
            //      This circumvents dependency checks (error is picked up by Safety system)
            GetEntityQuery(ComponentType.ReadOnly<CellTypeElement>());
        }

        protected override void OnUpdate()
        {
            EntityCommandBuffer.Concurrent ecb = _entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            
            Entity grid = GetSingletonEntity<Grid>();
            Grid gridComponent = EntityManager.GetComponentData<Grid>(grid);
            int2 gridSize = gridComponent.Size;
            
            DynamicBuffer<CellTypeElement> cellTypeBuffer = EntityManager.GetBuffer<CellTypeElement>(grid);
            ComponentDataFromEntity<CellPosition> cellPositionAccessor = GetComponentDataFromEntity<CellPosition>();
            
            Entities
                .WithAll<Farmer_Tag>()
                .WithAll<TillField_Intent>()
                .WithNativeDisableParallelForRestriction(cellTypeBuffer)
                .WithNativeDisableParallelForRestriction(cellPositionAccessor)
                .WithReadOnly(cellTypeBuffer)
                .ForEach((int entityInQueryIndex, Entity entity, in Target target) =>
                {
                    CellPosition cp = cellPositionAccessor[target.Value];
                    int index = (int) (cp.Value.x * gridSize.x + cp.Value.y);

                    if (cellTypeBuffer[index].Value == CellType.Tilled)
                    {
                        ecb.RemoveComponent<Target>(entityInQueryIndex, entity);
                    }
                }).ScheduleParallel();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}