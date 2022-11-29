using System;
using System.Collections.Generic;
using Ported.Scripts.Components;
using Ported.Scripts.Utils;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace Ported.Scripts.Systems
{
    [BurstCompile]
    public partial struct CollisionSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // List<Tuple<Entity, Entity>> collisions = new List<Tuple<Entity, Entity>>(); // CatEntity, RatEntity // todo : remove ...
            var beginBuffer = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);            
            int numCollisions = 0;
            
            foreach (var (positionComponentCat, _, catEntity) in SystemAPI.Query<RefRO<PositionComponent>,RefRO<CatComponent>>()
                         .WithEntityAccess())
            {
                // detection
                var catPos = positionComponentCat.ValueRO.position;
                var catSize = 1.0f;
                
                foreach (var (positionComponentRat, _, ratEntity) in SystemAPI
                             .Query<RefRO<PositionComponent>, RefRO<RatComponent>>()
                             .WithEntityAccess())
                {
                    var ratPos = positionComponentRat.ValueRO.position;
                    var ratSize = 1.0f;

                    // todo : need 'bounds/size' of an object
                    if (RatLabHelper.CollidesAABB(catPos, ratPos, catSize, ratSize))
                    {
                        numCollisions++;
                        // beginBuffer.DestroyEntity(ratEntity); // todo
                        // cat ate rat : todo
                    }
                }
            }

            // Debug.Log($"[CollisionSystem] destroyed entities: {numCollisions}");
        }
    }
}