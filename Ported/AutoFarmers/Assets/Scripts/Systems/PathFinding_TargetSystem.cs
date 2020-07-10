using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace AutoFarmers
{
    public class PathFindingTargetSystem : SystemBase
    {
        private EntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            EntityCommandBuffer ecb = _entityCommandBufferSystem.CreateCommandBuffer();

            bool usePathFinding = true;
            if (usePathFinding)
            {
                Entities
                    .WithNone<Target>()
                    .ForEach((Entity entity, ref PathFindingTarget pfTarget, in Translation translation) =>
                    {
                        int2 currentCoords = new int2((int)translation.Value.x, (int)translation.Value.z);
                        int2 targetCoords = new int2((int)GetComponent<Translation>(pfTarget.Value).Value.x, (int)GetComponent<Translation>(pfTarget.Value).Value.z);

                        bool alreadyInTargetCell = currentCoords.x == targetCoords.x && currentCoords.y == targetCoords.y;
                        if (alreadyInTargetCell)
                        {
                            ecb.AddComponent<Target>(entity, new Target { Value = pfTarget.Value });
                        }
                        else
                        {
                            // Set Target to current entity, PathFinding_NextStepSystem will take over
                            // (preferred over calculating a "next intermediate target" both here and in the _NextStepSystem)
                            ecb.AddComponent<Target>(entity, new Target { Value = entity });
                        }
                    }).Schedule();
            }
            else
            {
                // Not pathfinding? Just put the PFTarget directly into the next Target
                // (_NextStep will do nothing if both have the same Entity value)
                Entities
                    .WithNone<Target>()
                    .WithAll<Translation>() // just so we're doing this on exactly the same archetype as the ForEach in the if-statement
                    .ForEach((Entity entity, ref PathFindingTarget pfTarget) =>
                    {
                        ecb.AddComponent<Target>(entity, new Target { Value = pfTarget.Value });
                    }).Schedule();
            }

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
