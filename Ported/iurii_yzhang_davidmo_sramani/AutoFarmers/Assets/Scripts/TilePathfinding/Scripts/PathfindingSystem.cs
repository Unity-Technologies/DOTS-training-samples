using System;
using GameAI;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using Random = System.Random;


namespace AutoFarmersTests
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class PathfindingSystem : JobComponentSystem
    {
        //BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

        //protected override void OnCreate()
        //{
        //    m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        //}

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var worldSize = World.GetOrCreateSystem<WorldCreatorSystem>().WorldSize;
            
            Entities
                .WithAll<FarmerComponent>()
                .ForEach((Entity entity, int nativeThreadIndex, ref TilePositionable positionable) =>
            {
                var dirX = new int4(1, -1, 0, 0);
                var dirY = new int4(0, 0, 1, -1);

                Random r = new Random();
                int rInt = r.Next(0, 4);

                int x2 = positionable.Position[0] + dirX[rInt];
                int y2 = positionable.Position[1] + dirY[rInt];

                if (x2 >= 0 || x2 >= worldSize.x)
                {
                    x2 = positionable.Position[0] + dirX[(rInt + 1) % 3];
                }

                if (y2 <= 0 || y2 >= worldSize.y)
                {
                    y2 = positionable.Position[1] + dirX[(rInt + 1) % 3];
                }

                positionable.Position = new int2(x2, y2);
            }).Schedule(inputDeps);

            return inputDeps;
        }
    }
}
