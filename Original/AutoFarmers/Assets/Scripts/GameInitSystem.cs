using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Scenes;

public struct Tile
{
    public Entity empty;
    public Entity tilled;
}

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(SceneSystemGroup))]
public class GameInitSystem:SystemBase
{
    public static NativeHashMap<uint2, Tile> groundTiles;
    public static uint2 mapSize;

    static Entity tilledTile;

    protected override void OnUpdate()
    {
        Entities.WithStructuralChanges().ForEach((Entity entity, in GroundData groundData) =>
        {
            groundTiles = new NativeHashMap<uint2, Tile>(groundData.fieldSizeX * groundData.fieldSizeY, Allocator.Persistent);
            tilledTile = groundData.tilledGroundEntity;
            mapSize = new uint2((uint)groundData.fieldSizeX, (uint)groundData.fieldSizeY);

            for(int y = 0;y < groundData.fieldSizeY;++y)
            {
                for(int x = 0;x < groundData.fieldSizeX;++x)
                {
                    Entity empty = Entity.Null, tilled;

                    tilled = EntityManager.Instantiate(groundData.tilledGroundEntity);
                    EntityManager.RemoveComponent<Translation>(tilled);
                    EntityManager.RemoveComponent<Rotation>(tilled);
                    EntityManager.RemoveComponent<Scale>(tilled);
                    EntityManager.RemoveComponent<NonUniformScale>(tilled);
                    EntityManager.AddComponentData<Position2D>(tilled, new Position2D { position = new Unity.Mathematics.float2(x,y) });
                    EntityManager.AddComponent<GroundTilledState>(tilled);

                    if (groundData.debugOptions == MapDebugOptions.Plants)
                        EntityManager.AddComponent<PlantedSeedTag>(tilled);
                    else if (groundData.debugOptions != MapDebugOptions.Tilled)
                    {
                        empty = EntityManager.Instantiate(groundData.defaultGroundEntity);
                        EntityManager.RemoveComponent<Translation>(empty);
                        EntityManager.RemoveComponent<Rotation>(empty);
                        EntityManager.RemoveComponent<Scale>(empty);
                        EntityManager.RemoveComponent<NonUniformScale>(empty);
                        EntityManager.AddComponentData<Position2D>(empty,new Position2D { position = new Unity.Mathematics.float2(x,y) });
                        EntityManager.AddComponent<Disabled>(tilled);
                    }

                    groundTiles[new uint2((uint)x, (uint)y)] = new Tile{ empty = empty, tilled = tilled };
                }
            }
            EntityManager.RemoveComponent<GroundData>(entity);
        }).Run();

        // Spawn stores
        Entities.WithStructuralChanges().ForEach((Entity entity,in StoreData_Spawner storeData) =>
        {
            for(int i = 0;i < storeData.count;i++)
            {
                int posX = UnityEngine.Random.Range(0,storeData.mapX);
                int posY = UnityEngine.Random.Range(0,storeData.mapY);
                var pos = new Unity.Mathematics.float2(posX,posY); //TODOJENNY

                var store = EntityManager.Instantiate(storeData.prefab);
                EntityManager.RemoveComponent<Translation>(store);
                EntityManager.RemoveComponent<Rotation>(store);
                EntityManager.RemoveComponent<Scale>(store);
                EntityManager.RemoveComponent<NonUniformScale>(store);
                EntityManager.AddComponentData<Position2D>(store,new Position2D { position = pos });
                EntityManager.AddComponentData<Store>(store, new Store 
                { 
                    nbPlantsSold = 0,
                    position = pos 
                });
            }

            EntityManager.RemoveComponent<StoreData_Spawner>(entity);
        }).Run();
    }

    protected override void OnDestroy()
    {
        groundTiles.Dispose();
    }
}