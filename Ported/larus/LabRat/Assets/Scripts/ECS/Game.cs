using ECSExamples;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class StartGame : ComponentSystem
{
    protected override void OnCreate()
    {
        //RequireSingletonForUpdate<GameStateComponent>();
        var entity = EntityManager.CreateEntity();
        EntityManager.AddComponent<GameStateComponent>(entity);
    }

    protected override void OnUpdate()
    {
        var gameConfig = GetSingleton<GameConfigComponent>();
        var gameState = GetSingleton<GameStateComponent>();

        // Reset game if game timer has expired
        if (HasSingleton<GameInProgressComponent>())
        {
            if (gameConfig.GameLength + gameState.StartTime < Time.time)
            {
                Debug.Log(">>>>>>>>> Game finished <<<<<<<<<<");
                CleanupLevel();
            }
            return;
        }
        // Wait a bit before starting another game (unless this is the first)
        if (gameState.StartTime > 0 && gameState.StartTime + gameConfig.GameLength + 5f > Time.time)
            return;

        SpawnPlayer(1);
        SpawnPlayer(2);
        SpawnPlayer(3);
        SpawnPlayer(4);
        gameState.StartTime = Time.time;
        EntityManager.SetComponentData(GetSingletonEntity<GameStateComponent>(), gameState);
        var entity = EntityManager.CreateEntity();
        EntityManager.AddComponent<GameInProgressComponent>(entity);

        // Restart, cleanup level but keep player data
        //CleanupLevel();
        //gameState.StartTime = Time.time;
        //EntityManager.SetComponentData(GetSingletonEntity<GameStateComponent>(), gameState);
    }

    private void CleanupLevel()
    {
        var entity = GetSingletonEntity<GameInProgressComponent>();
        EntityManager.DestroyEntity(entity);
        var cleanup = GetEntityQuery(typeof(EatenComponentTag)).ToEntityArray(Allocator.TempJob);
        for (int i = 0; i < cleanup.Length; ++i)
            EntityManager.DestroyEntity(cleanup[i]);
        cleanup.Dispose();
        cleanup = GetEntityQuery(typeof(EaterComponentTag)).ToEntityArray(Allocator.TempJob);
        for (int i = 0; i < cleanup.Length; ++i)
            EntityManager.DestroyEntity(cleanup[i]);
        cleanup.Dispose();
        cleanup = GetEntityQuery(typeof(OverlayColorComponent)).ToEntityArray(Allocator.TempJob);
        for (int i = 0; i < cleanup.Length; ++i)
            EntityManager.SetComponentData(cleanup[i], new Translation{Value = new float3(0,-10f,-10f)});
        cleanup.Dispose();
        cleanup = GetEntityQuery(typeof(OverlayComponentTag)).ToEntityArray(Allocator.TempJob);
        for (int i = 0; i < cleanup.Length; ++i)
            EntityManager.SetComponentData(cleanup[i], new Translation{Value = new float3(0,-10f,-10f)});
        cleanup.Dispose();
        var boardSystem = World.GetExistingSystem<BoardSystem>();
        var cellMap = boardSystem.CellMap;
        var cellKeys = cellMap.GetKeyArray(Allocator.TempJob);
        for (int i = 0; i < cellKeys.Length; ++i)
        {
            var data = cellMap[cellKeys[i]];
            data.data &= ~CellData.Arrow;
            cellMap[cellKeys[i]] = data;
        }
        cellKeys.Dispose();
        boardSystem.ArrowMap.Clear();
        // TODO: reset score etc
    }

    public void SpawnPlayer(int playerId)
    {
        var prefab = GetSingleton<PrefabCollectionComponent>().PlayerPrefab;
        var player = EntityManager.Instantiate(prefab);
        PostUpdateCommands.AddComponent(player, new PlayerComponent{PlayerId = playerId});
        PostUpdateCommands.AddBuffer<PlayerInput>(player);

        if (playerId == 1)
            PostUpdateCommands.AddComponent(player, new LocalPlayerComponent{PlayerId = playerId});
        else
            PostUpdateCommands.AddComponent<AiPlayerComponent>(player);

        // Set up the players home base
        World.GetExistingSystem<BoardSystem>().SpawnHomebase(playerId);

        // Set up links to this players overlays
        var overlayEntities = GetEntityQuery(typeof(OverlayComponentTag), typeof(Translation)).ToEntityArray(Allocator.TempJob);
        var overlayColorEntities = GetEntityQuery(typeof(OverlayColorComponent), typeof(Translation)).ToEntityArray(Allocator.TempJob);
        var startIndex = (playerId-1) * PlayerConstants.MaxArrows;
        PostUpdateCommands.AddComponent(player, new PlayerOverlayComponent
        {
            overlay0 = overlayEntities[startIndex],
            overlay1 = overlayEntities[startIndex+1],
            overlay2 = overlayEntities[startIndex+2],
            overlayColor0 = overlayColorEntities[startIndex],
            overlayColor1 = overlayColorEntities[startIndex+1],
            overlayColor2 = overlayColorEntities[startIndex+2]
        });
        overlayEntities.Dispose();
        overlayColorEntities.Dispose();
    }
}