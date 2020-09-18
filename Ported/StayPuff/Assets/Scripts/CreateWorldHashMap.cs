using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;

[UpdateAfter(typeof(BuildingCreate))]
public class CreateWorldHashMap : SystemBase
{
    NativeHashMap<uint, SharedWorldBounds> worldBoundsHashMap;
    static float cellSize;
    
    static uint GetHashCode(float2 position, float hashCellSize)
    {
        return math.hash(new int2(math.floor(position / hashCellSize)));
    }
    
    protected override void OnStartRunning()
    {
        if(worldBoundsHashMap.IsCreated)
            return;

        WorldHashData hashProperties = GetSingleton<WorldHashData>();
        cellSize = hashProperties.cellSize;
        
        int width = hashProperties.gridSteps.x;
        int height = hashProperties.gridSteps.y;
        int halfWidth = width / 2;
        int halfHeight = width / 2;
        float halfCellSize = cellSize * 0.5f;

        worldBoundsHashMap = new NativeHashMap<uint, SharedWorldBounds>(width * height, Allocator.TempJob);

        for(int x = -halfWidth; x < width; x++)
        {
            for(int y = -halfHeight; y < height; y++)
            {
                float2 gridPosition;
                gridPosition.x = x * cellSize;
                gridPosition.y = y * cellSize;

                uint hash = GetHashCode(gridPosition , cellSize);

                worldBoundsHashMap.TryAdd(hash, new SharedWorldBounds { center = new float3(gridPosition.x, 0.0f, gridPosition.y) });
            }
        }

    }

    protected override void OnDestroy()
    {
        if(worldBoundsHashMap.IsCreated)
        {
            worldBoundsHashMap.Dispose();
        }
    }

    protected override void OnUpdate()
    {
        //Gather all entities (not tornados)
        EntityQuery translationQuery = GetEntityQuery(typeof(Translation), typeof(IncludeInSharedWorldBounds));
        NativeArray<Entity> entityArray = translationQuery.ToEntityArray(Allocator.TempJob);
        NativeArray<Translation> translationArray = translationQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

        //Hash entities into shared world bounds
        for(int i = 0; i < entityArray.Length; i++)
        {
            Translation translation = translationArray[i];
            Entity entity = entityArray[i];

            uint hash = GetHashCode(new float2(translation.Value.x, translation.Value.z) , cellSize);
            SharedWorldBounds worldBounds;
            if (worldBoundsHashMap.TryGetValue(hash, out worldBounds))
            {
                EntityManager.AddSharedComponentData<SharedWorldBounds>(entity, worldBounds);
            }
        }
        EntityManager.AddComponent<PhysicsExclude>(translationQuery);
        EntityManager.RemoveComponent<IncludeInSharedWorldBounds>(translationQuery);
        
        entityArray.Dispose();
        translationArray.Dispose();
    }
}
