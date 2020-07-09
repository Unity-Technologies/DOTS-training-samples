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
            ComponentDataFromEntity<LocalToWorld> localToWorldAccessor = GetComponentDataFromEntity<LocalToWorld>();
            
            Entities
                .WithAll<Farmer_Tag>()
                .WithAll<TillField_Intent>()
                .WithNativeDisableParallelForRestriction(cellTypeBuffer)
                .WithNativeDisableParallelForRestriction(localToWorldAccessor)
                .WithReadOnly(cellTypeBuffer)
                .ForEach((int entityInQueryIndex, Entity entity, in Target target) =>
                {
                    LocalToWorld l2w = localToWorldAccessor[target.Value];
                    int index = (int) (l2w.Position.x * gridSize.x + l2w.Position.z);

                    if (cellTypeBuffer[index].Value == CellType.Tilled)
                    {
                        ecb.RemoveComponent<Target>(entityInQueryIndex, entity);
                    }
                }).ScheduleParallel();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}