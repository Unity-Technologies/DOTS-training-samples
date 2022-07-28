using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

[BurstCompile]
[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial struct PlayerSpawner : ISystem
{
    // Queries should not be created on the spot in OnUpdate, so they are cached in fields.
    EntityQuery playerQuery;
    EntityQuery boxQuery;
    ComponentDataFromEntity<Boxes> boxesFromEntity;
    //ComponentDataFromEntity<Translation> playerCFromEntity;


    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // This system should not run before the Config singleton has been loaded.
        state.RequireForUpdate<Config>();
        boxesFromEntity = state.GetComponentDataFromEntity<Boxes>(true);
        //playerCFromEntity= state.GetComponentDataFromEntity<Translation>(true);
        boxQuery = state.GetEntityQuery(typeof(Boxes));
        playerQuery = state.GetEntityQuery(typeof(PlayerComponent));
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        boxesFromEntity.Update(ref state);
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var boxEntities = boxQuery.ToEntityArray(Unity.Collections.Allocator.TempJob);
        var players = CollectionHelper.CreateNativeArray<Entity>(1, Allocator.Temp);
        ecb.Instantiate(config.playerPrefab, players);

        UnityEngine.Debug.Log("Before the Masking" );
        var queryMask = playerQuery.GetEntityQueryMask();
        UnityEngine.Debug.Log("Post Masking" );

        foreach (var player in players)
        {
            ecb.SetComponentForLinkedEntityGroup(player, queryMask, PlayerProperties(boxesFromEntity, config, boxEntities));
            UnityEngine.Debug.Log("An endless cycle of life and death" );
        }
        // This system should only run once at startup. So it disables itself after one update.
        state.Enabled = false;
        UnityEngine.Debug.Log("I only happen once");
    }

    public static PlayerComponent PlayerProperties(ComponentDataFromEntity<Boxes> boxesFromEntity, Config config, NativeArray<Entity> boxes)
    {
        UnityEngine.Debug.Log("No boxes :(((");
        var pc = new PlayerComponent();
        foreach (Entity box in boxes){
            UnityEngine.Debug.Log("we have a box");
            pc.startBox = box;
            Boxes boxRef = boxesFromEntity[box]; 
            float3 pcPos = Spawn(boxRef.row,boxRef.column, pc , config, boxRef, boxesFromEntity, boxes);
            //playerCFromEntity[box] = new Translation(0,0,0);
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