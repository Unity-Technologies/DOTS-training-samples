using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace AutoFarmers
{
    public class PathFinding_NextStepSystem : SystemBase
    {
        private EntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

            GetEntityQuery(ComponentType.ReadOnly<CellEntityElement>());
            GetEntityQuery(ComponentType.ReadOnly<CellTypeElement>());
        }

        protected override void OnUpdate()
        {
            EntityCommandBuffer ecb = _entityCommandBufferSystem.CreateCommandBuffer();

            Entity gridEntity = GetSingletonEntity<Grid>();
            Grid grid = EntityManager.GetComponentData<Grid>(gridEntity);
            DynamicBuffer<CellEntityElement> cellEntityBuffer = EntityManager.GetBuffer<CellEntityElement>(gridEntity);
            DynamicBuffer<CellTypeElement> cellTypeBuffer = EntityManager.GetBuffer<CellTypeElement>(gridEntity);

            // Target.Value == PFTarget.Value => nothing to do (either pathfinding isn't used, or we're in the last line towards the target)
            // Target.Value != PFTarget.Value => this means pathfinding is active and has chosen an intermediate target to move towards
            Entities
                .WithReadOnly(cellEntityBuffer)
                .WithReadOnly(cellTypeBuffer)
                .ForEach((Entity entity, in PathFindingTarget pfTarget, in Target intermediateTarget, in Translation translation) =>
                {
                    if (pfTarget.Value == intermediateTarget.Value)
                        return;

                    int2 currentCoords = new int2((int)translation.Value.x, (int)translation.Value.z);
                    int2 intermediateTargetCoords = new int2((int)GetComponent<Translation>(intermediateTarget.Value).Value.x, (int)GetComponent<Translation>(intermediateTarget.Value).Value.z);
                    if (math.distance(currentCoords, intermediateTargetCoords) > 0.1f)
                    {
                        // Don't do anything yet ...
                        // TODO: bad for parallelism?
                        return;
                    }
                    int2 targetCoords = new int2((int)GetComponent<Translation>(pfTarget.Value).Value.x, (int)GetComponent<Translation>(pfTarget.Value).Value.z);
                    int dirX = targetCoords.x > currentCoords.x ? 1 : -1;
                    int dirY = targetCoords.y > currentCoords.y ? 1 : -1;

                    bool openLineToTarget = false;
                    int longestX = 0;
                    bool foundStopX = false;
                    while (!foundStopX && !openLineToTarget)
                    {
                        int nextX = currentCoords.x + dirX * (longestX + 1);
                        int nextIdx = nextX * grid.Size.x + currentCoords.y;

                        if (nextX == targetCoords.x && currentCoords.y == targetCoords.y)
                        {
                            openLineToTarget = true;
                        }
                        else if (nextX < 0 || nextX >= grid.Size.x || cellTypeBuffer[nextIdx].Value == CellType.Rock)
                        {
                            foundStopX = true;
                        }
                        else
                        {
                            longestX++;
                            if (nextX == targetCoords.x)
                            {
                                foundStopX = true;
                            }
                        }
                    }

                    int longestY = 0;
                    bool foundStopY = false;
                    while (!foundStopY && !openLineToTarget)
                    {
                        int nextY = currentCoords.y + dirY * (longestY + 1);
                        int nextIdx = currentCoords.x * grid.Size.x + nextY;

                        if (currentCoords.x == targetCoords.x && nextY == targetCoords.y)
                        {
                            openLineToTarget = true;
                        }
                        else if (nextY < 0 || nextY >= grid.Size.y || cellTypeBuffer[nextIdx].Value == CellType.Rock)
                        {
                            // Blocked by rock or end of farm
                            foundStopY = true;
                        }
                        else
                        {
                            // Go one step further
                            longestY++;
                            if (nextY == targetCoords.y)
                            {
                                foundStopY = true;
                            }
                        }
                    }

                    if (openLineToTarget)
                    {
                        // final stretch
                        ecb.SetComponent<Target>(entity, new Target { Value = pfTarget.Value });
                    }
                    else
                    {
                        if (longestY > longestX)
                            dirX = 0;
                        else
                            dirY = 0;

                        int idx = (currentCoords.x + dirX * longestX) * grid.Size.x + (currentCoords.y + dirY * longestY);
                        Entity nextIntermediateTarget = cellEntityBuffer[idx].Value;
                        ecb.SetComponent<Target>(entity, new Target { Value = nextIntermediateTarget });
                    }
                }).Schedule();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
