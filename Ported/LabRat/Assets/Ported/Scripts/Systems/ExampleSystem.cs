// using System.Collections.Generic;
// using Unity.Burst;
// using Unity.Collections;
// using Unity.Entities;
//
// namespace Ported.Scripts.Systems
// {
//     // by default it's recommended to always opt in to using BurstCompile, required both for the EcsSystem [struct] and each function
//     // and then manually opt out of using BurstCompile if we need to access managed memory
//     [BurstCompile]
//     public partial struct ExampleSystem : ISystem
//     {
//         // this is the old way of working with entities that forces you to manually deal with 'per chunk' dimensionality
//         // the new way is to do a foreach inside the update function
//         // private EntityQuery m_moveableQuery;
//         
//         
//         [BurstCompile]
//         public void OnCreate(ref SystemState state)
//         {
//             // requires manual chunk handling foreach entity [favorable to use SystemAPI.Query<...>]
//             // m_moveableQuery = SystemAPI.QueryBuilder().WithAll<UnitMovementComponent, PositionComponent>().Build();

                // When looking for information/commands to invoke, these are the 'suggested order of where to look'
                // 1st) SystemAPI
                // 2nd) SystemState
                // 3rd) state.EntityManager
                // 4th) state.WorldUnmanaged / state.world
//         }
//
//         [BurstCompile]
//         public void OnDestroy(ref SystemState state)
//         {
//         }
//
//         [BurstCompile]
//         public void OnUpdate(ref SystemState state)
//         {
//             // we can fetch the default ecb (EntityCommandBuffer) that will automatically playback the commands during it's sync points
//             var beginBuffer = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
//             var endBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
//             
//             // or we can create our own buffer if we want to manually control the playback
//             var manualBuffer = new EntityCommandBuffer(Allocator.Temp);
//
//             // this is a 'query-helper' that will automatically allow us to iterate over all entities matching our query
//             // we specify what type of access we need to each component (r/rw), [WithEntityAccess] makes sure we get back the entity handle under iteration
//             foreach (var (move, pos, entity) in SystemAPI
//                          .Query<RefRO<UnitMovementComponent>, RefRW<PositionComponent>>().WithEntityAccess())
//             {
//                 pos.ValueRW.position += (move.ValueRO.speed * move.ValueRO.direction);
//
//                 if (pos.ValueRW.position.x > 1338)
//                 {
//                     // endBuffer.DestroyEntity(entity); // depending on when we want to destroy this entity
//                     // beginBuffer.DestroyEntity(entity); // depending on when we want to destroy this entity
//                     // keep in mind that this entity handle
//                     // manualBuffer.DestroyEntity(entity);
//                     // state.EntityManager.DestroyEntity(entity); // command buffer is wrapping this command, can't perform this directly due to modifying container we are iterating over
//                 }
//             }
//
//             // manualBuffer.Playback(state.EntityManager); // for the manual buffer we have to manually invoke playback
//         }
//     }
// }