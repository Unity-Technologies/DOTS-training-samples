using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

[BurstCompile]
[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial struct BoxSpawningSystem : ISystem
{
    // Queries should not be created on the spot in OnUpdate, so they are cached in fields.
    EntityQuery m_BaseColorQuery;

    // Player Spawning
    EntityQuery playerQuery;
    EntityQuery boxQuery;
    ComponentDataFromEntity<Boxes> boxesFromEntity;
    ComponentDataFromEntity<PlayerComponent> pcFromEntity;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // This system should not run before the Config singleton has been loaded.
        state.RequireForUpdate<Config>();

        // Player values
        boxesFromEntity = state.GetComponentDataFromEntity<Boxes>(true);
        pcFromEntity= state.GetComponentDataFromEntity<PlayerComponent>(true);

        var queryBuilderBoxes = new EntityQueryDescBuilder(Allocator.Temp);
        queryBuilderBoxes.AddAll(ComponentType.ReadWrite<Boxes>());
        queryBuilderBoxes.FinalizeQuery(); 
        boxQuery = state.GetEntityQuery(queryBuilderBoxes);

        var queryBuilderPlayer = new EntityQueryDescBuilder(Allocator.Temp);
        queryBuilderPlayer.AddAll(ComponentType.ReadWrite<PlayerComponent>());
        queryBuilderPlayer.FinalizeQuery(); 
        playerQuery = state.GetEntityQuery(queryBuilderPlayer);

        queryBuilderBoxes.Dispose();
        queryBuilderPlayer.Dispose();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();

        if (boxQuery.CalculateEntityCount() == 0){
            
            var ecbBoxSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecbBox = ecbBoxSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            var boxes = CollectionHelper.CreateNativeArray<Entity>(config.boxCount, Allocator.Temp);

            var boxQueryMask = boxQuery.GetEntityQueryMask();

            ecbBox.Instantiate(config.boxPrefab, boxes);

            boxesFromEntity.Update(ref state);

            int row = 0;
            int col = 0;
            foreach (var box in boxes)
            {
                Boxes baseBox = boxesFromEntity[config.boxPrefab];
                ecbBox.SetComponentForLinkedEntityGroup(box, boxQueryMask, BoxProperties(row, col, baseBox));
                if (row >= config.terrainWidth)
                {
                    row = 0;
                    col++;
                }
                else
                    row++;
            }
            boxes.Dispose();


        } else {
            var ecbPlayerSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecbPlayer = ecbPlayerSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            var boxEntities = boxQuery.ToEntityArray(Unity.Collections.Allocator.TempJob);
            var players = CollectionHelper.CreateNativeArray<Entity>(1, Allocator.Temp);
            var playerQueryMask = playerQuery.GetEntityQueryMask();

            ecbPlayer.Instantiate(config.playerPrefab, players);
            //ecb.Playback(state.WorldUnmanaged.EntityManager);

            boxesFromEntity.Update(ref state);
            pcFromEntity.Update(ref state);

            foreach (var player in players)
            {
                PlayerComponent pc = pcFromEntity[config.playerPrefab];
                ecbPlayer.SetComponentForLinkedEntityGroup(player, playerQueryMask, PlayerProperties(boxesFromEntity, config, boxEntities, pc));
            }

            state.Enabled = false;

            players.Dispose();
            boxEntities.Dispose();
        }
        //ecbBox.Playback(state.WorldUnmanaged.EntityManager);

    }

    public static Boxes BoxProperties(int row, int col, Boxes box){
        //UnityEngine.Debug.Log(row);
        Boxes newBox = box;
        newBox.row = row; 
        newBox.column = col;
        return newBox; 
    }

    public static PlayerComponent PlayerProperties(ComponentDataFromEntity<Boxes> boxesFromEntity, Config config, NativeArray<Entity> boxes, PlayerComponent prefabData)
    {
        var pc = prefabData;
        pc.para.paraA = 1;
        foreach (Entity box in boxes){
            pc.startBox = box;
            pc.endBox = box;
            Boxes boxRef = boxesFromEntity[box]; 
            float3 pcPos = Spawn(boxRef.row,boxRef.column, pc , config, boxRef, boxesFromEntity, boxes);
            return pc;
        }
        return pc;
    }

    public static float3 Spawn(int col, int row, PlayerComponent playerComponent, 
    Config config, Boxes startBox, ComponentDataFromEntity<Boxes> boxesFromEntity, NativeArray<Entity> boxes){
        Boxes newStartBox; 
        Entity newStartBoxEntity = new Entity(); 
        
        foreach (var box in boxes) { 
            newStartBox = boxesFromEntity[box];
            if (newStartBox.row == row && newStartBox.column == col){
                newStartBoxEntity = box;
                break; 
            }
        }
        playerComponent.startBox = newStartBoxEntity;
		playerComponent.endBox = playerComponent.startBox;

        float top = startBox.top; 
        return TerrainAreaClusters.LocalPositionFromBox(col, row, config, top + playerComponent.yOffset);
    }
    
}