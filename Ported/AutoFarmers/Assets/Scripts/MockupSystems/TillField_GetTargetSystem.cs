using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace AutoFarmers
{
    public class TillField_GetTargetSystem : SystemBase
    {
        private EntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityQuery _CellsQuery;

        protected override void OnCreate()
        {
            _CellsQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadOnly<Cell>()
                }
            });

            _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            EntityCommandBuffer.Concurrent ecb = _entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            
            var cells =
                _CellsQuery.ToComponentDataArrayAsync<Cell>(Allocator.TempJob, out var cellHandle);
            
            Entity grid = GetSingletonEntity<Grid>();
            Grid gridComponent = EntityManager.GetComponentData<Grid>(grid);
            int2 gridSize = gridComponent.Size;
            DynamicBuffer<CellTypeElement> cellTypeBuffer = EntityManager.GetBuffer<CellTypeElement>(grid);
            DynamicBuffer<CellEntityElement> cellEntityBuffer = EntityManager.GetBuffer<CellEntityElement>(grid);
            
            Entities
                .WithAll<Farmer_Tag>()
                .WithAll<TillField_Intent>()
                .WithNone<Target>()
                .WithNativeDisableParallelForRestriction(cellTypeBuffer)
                .WithNativeDisableParallelForRestriction(cellEntityBuffer)
                .ForEach((int entityInQueryIndex, Entity entity, ref Translation translation, in TillRect tillRect) =>
                {
                    int PosX = (int) (translation.Value.x);
                    int PosY = (int) (translation.Value.z);

                    //TODO: This feels dirty
                    Entity targetEntity = default(Entity);
                    
                    //NOTE: Get first non-tilled cell
                    bool hasTarget = false;
                    for (int gridX = 0; gridX < tillRect.X; gridX++)
                    {
                        if (hasTarget) break;
                            
                        for (int gridY = 0; gridY < tillRect.Y; gridY++)
                        {
                            if (hasTarget) break;
                            
                            int index = gridX * gridSize.x + gridY;
                            if (cellTypeBuffer[index].Value != CellType.Tilled)
                            {
                                targetEntity = cellEntityBuffer[index].Value;
                                hasTarget = true;
                            }
                        }
                    }

                    if (hasTarget)
                    {
                        ecb.AddComponent(entityInQueryIndex, entity, new Target()
                        {
                            Value = targetEntity
                        });
                    }
                }).ScheduleParallel();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
