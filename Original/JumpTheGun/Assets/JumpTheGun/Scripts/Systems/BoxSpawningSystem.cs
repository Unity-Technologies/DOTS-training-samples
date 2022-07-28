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
        m_BaseColorQuery = state.GetEntityQuery(ComponentType.ReadOnly<URPMaterialPropertyBaseColor>());

        // Player values
        boxesFromEntity = state.GetComponentDataFromEntity<Boxes>(true);
        pcFromEntity= state.GetComponentDataFromEntity<PlayerComponent>(true);
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
        var random = Random.CreateFromIndex(1234);
        var hue = random.NextFloat();

        URPMaterialPropertyBaseColor RandomColor()
        {
            hue = (hue + 0.618034005f) % 1;
            var color = UnityEngine.Color.HSVToRGB(hue, 5.0f, 2.3f);
            return new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)color };
        }

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var boxes = CollectionHelper.CreateNativeArray<Entity>(config.boxCount, Allocator.Temp);
        ecb.Instantiate(config.boxPrefab, boxes);

        var queryMask = m_BaseColorQuery.GetEntityQueryMask();

        foreach (var box in boxes)
        {
            ecb.SetComponentForLinkedEntityGroup(box, queryMask, RandomColor());
        }
        ecb.Playback(state.WorldUnmanaged.EntityManager);

        var configPlayer = SystemAPI.GetSingleton<Config>();
        var ecbPlayerSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecbPlayer = ecbPlayerSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var boxEntities = boxQuery.ToEntityArray(Unity.Collections.Allocator.TempJob);
        var players = CollectionHelper.CreateNativeArray<Entity>(1, Allocator.Temp);
        var playerQueryMask = playerQuery.GetEntityQueryMask();

        ecbPlayer.Instantiate(configPlayer.playerPrefab, players);
        //ecb.Playback(state.WorldUnmanaged.EntityManager);

        boxesFromEntity.Update(ref state);
        pcFromEntity.Update(ref state);

        foreach (var player in players)
        {
            PlayerComponent pc = pcFromEntity[config.playerPrefab];
            ecbPlayer.SetComponentForLinkedEntityGroup(player, playerQueryMask, PlayerProperties(boxesFromEntity, config, boxEntities, pc));
        }
        state.Enabled = false;
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