using DOTSRATS;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using Rotation = DOTSRATS.Rotation;

public class SetupSystem : SystemBase
{
    protected override void OnUpdate()
    {
        const float k_yRangeSize = 0.03f;
        
        var random = Random.CreateFromIndex((uint)System.DateTime.Now.Ticks);
        var gameState = GetSingleton<GameState>();

        Entities
            .WithStructuralChanges()
            .WithAll<BoardSpawner>()
            .ForEach((Entity entity, in BoardSpawner boardSpawner) =>
            {
                var size = gameState.boardSize;
                
                // Spawn tiles
                for (int z = 0; z < size; ++z)
                {
                    for (int x = 0; x < size; ++x)
                    {
                        var tile = EntityManager.Instantiate(boardSpawner.tilePrefab);
                        var yValue = random.NextFloat(-k_yRangeSize, k_yRangeSize);
                        var translation = new Translation() { Value = new float3(x, yValue - 0.5f, z) };
                        EntityManager.SetComponentData<Translation>(tile, translation);
                    }
                }

                EntityManager.DestroyEntity(entity);
            }).Run();
    }
}
