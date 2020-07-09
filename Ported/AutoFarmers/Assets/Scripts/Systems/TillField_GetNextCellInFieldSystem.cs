using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace AutoFarmers
{
    public class TillField_GetNextCellInFieldSystem : SystemBase
    {
        private EntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
            
            GetEntityQuery(ComponentType.ReadOnly<CellTypeElement>());
        }

        protected override void OnUpdate()
        {
            EntityCommandBuffer.Concurrent ecb = _entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            
            Entity grid = GetSingletonEntity<Grid>();
            Grid gridComponent = EntityManager.GetComponentData<Grid>(grid);
            int2 gridSize = gridComponent.Size;
            
            DynamicBuffer<CellTypeElement> cellTypeBuffer = EntityManager.GetBuffer<CellTypeElement>(grid);
            DynamicBuffer<CellEntityElement> cellEntityBuffer = EntityManager.GetBuffer<CellEntityElement>(grid);
            
            Entities
                .WithAll<Farmer_Tag>()
                .WithAll<TillField_Intent>()
                .WithNone<Cooldown>()
                .WithNone<Target>()
                .WithNativeDisableParallelForRestriction(cellTypeBuffer)
                .WithNativeDisableParallelForRestriction(cellEntityBuffer)
                .WithReadOnly(cellTypeBuffer)
                .WithReadOnly(cellEntityBuffer)
                .ForEach((int entityInQueryIndex, Entity entity, in TillRect tillRect) =>
                {
                    //TODO: This feels dirty
                    Entity targetEntity = default(Entity);
                    
                    //NOTE: Get first non-tilled cell
                    bool hasTarget = false;
                    for (int gridX = tillRect.X; gridX < tillRect.X + tillRect.Width; gridX++)
                    {
                        if (hasTarget) break;
                            
                        for (int gridY = tillRect.Y; gridY < tillRect.Y + tillRect.Height; gridY++)
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
                    else
                    {
                        // Tilled whole rect, get a new intent
                        ecb.RemoveComponent<TillField_Intent>(entityInQueryIndex, entity);
                        ecb.RemoveComponent<TillRect>(entityInQueryIndex, entity);
                    }
                }).ScheduleParallel();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
