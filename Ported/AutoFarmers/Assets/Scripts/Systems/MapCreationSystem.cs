 
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct MapCreationSystem : ISystem
    {
         EntityQuery cellQuery;
 
         public void OnCreate(ref SystemState state)
         {
             state.RequireForUpdate<Map>();
             
             cellQuery = state.GetEntityQuery(ComponentType.ReadOnly<Cell>());
               var entity = state.EntityManager.CreateEntity(ComponentType.ReadOnly<Grid>());
              state.EntityManager.SetName(entity,"Grid");
              state.EntityManager.AddBuffer<CellType>(entity);
              state.EntityManager.AddBuffer<CellEntity>(entity);
         }

         public void OnDestroy(ref SystemState state)
         {
         }

         public void OnUpdate(ref SystemState state)
         {
             var map = SystemAPI.GetSingleton<Map>();
             
             var entity = SystemAPI.GetSingletonEntity<Grid>();
             var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
             var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
             
             var cells = CollectionHelper.CreateNativeArray<Entity>(map.mapSize.x*map.mapSize.y, Allocator.Temp);
             ecb.Instantiate(map.cellPrefab, cells);
             Grid grid = new Grid {size = map.mapSize};
             foreach (var cell in cells)
             {
                  
                 ecb.SetComponent(entity, grid);
                  
                 var cellCount = map.mapSize.x * map.mapSize.y;
                 var typeBuffer = SystemAPI.GetSingletonBuffer<CellType>();
                 typeBuffer.ResizeUninitialized(cellCount);
                 for (var i = 0; i < cellCount; i++)
                 {
                     typeBuffer[i] = new CellType(CellState.Raw);
                 }
                 
                 var entityBuffer = SystemAPI.GetSingletonBuffer<CellEntity>();
                 entityBuffer.ResizeUninitialized(cellCount);
                 for (var i = 0; i < cellCount; i++)
                 {
                     entityBuffer[i] = new CellEntity(cell);
                 }
                 
             }
             for (var x = 0; x < map.mapSize.x; x++)
             {
                 for (var y = 0; y < map.mapSize.y; y++)
                 {
                     var i = x * map.mapSize.y + y;
                     ecb.SetComponent(cells[i], new Translation() { Value = math.float3(x, 0, y) + new float3(0.5f, 0, 0.5f)});
                     ecb.SetComponent(cells[i], new CellPosition() { cellPosition = new int2(x, y)} );
                 }
             }
             cells.Dispose();

             state.Enabled = false;
         }
    }