using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine;
using Unity.Collections;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class GameInitSystem:SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithStructuralChanges().ForEach((Entity entity, in GroundData groundData) =>
        {
            // ref var d = blob.Value.storePositions;
            for(int y = 0;y < groundData.fieldSizeY;++y)
            {
                for(int x = 0;x < groundData.fieldSizeX;++x)
                {
                    var tile = EntityManager.Instantiate(groundData.debugOptions == MapDebugOptions.Tilled || groundData.debugOptions == MapDebugOptions.Plants ? groundData.tilledGroundEntity : groundData.defaultGroundEntity);
                    EntityManager.RemoveComponent<Translation>(tile);
                    EntityManager.RemoveComponent<Rotation>(tile);
                    EntityManager.RemoveComponent<Scale>(tile);
                    EntityManager.RemoveComponent<NonUniformScale>(tile);
                    EntityManager.AddComponentData<Position2D>(tile,new Position2D { position = new Unity.Mathematics.float2(x,y) });

                    var renderer = EntityManager.GetSharedComponentData<RenderMesh>(tile);

                    switch (groundData.debugOptions)
                    {
                        case MapDebugOptions.Empty:
                            continue;
                        case MapDebugOptions.Tilled:
                            EntityManager.AddComponent<GroundTilledState>(tile);
                            break;
                        case MapDebugOptions.Plants:
                            EntityManager.AddComponent<PlantedSeedTag>(tile);
                            EntityManager.AddComponent<GroundTilledState>(tile);
                            break;
                    }
                }
            }
            EntityManager.RemoveComponent<GroundData>(entity);
        }).Run();

        // Spawn stores
        Entities.WithStructuralChanges().ForEach((Entity entity,in StoreData_Spawner storeData) =>
        {
            for(int i = 0;i < storeData.count;i++)
            {
                int posX = Random.Range(0,storeData.mapX);
                int posY = Random.Range(0,storeData.mapY);

                var store = EntityManager.Instantiate(storeData.prefab);
                EntityManager.RemoveComponent<Translation>(store);
                EntityManager.RemoveComponent<Rotation>(store);
                EntityManager.RemoveComponent<Scale>(store);
                EntityManager.RemoveComponent<NonUniformScale>(store);
                EntityManager.AddComponentData<Position2D>(store,new Position2D { position = new Unity.Mathematics.float2(posX,posY) });
                EntityManager.AddComponent<Store>(store);
            }


            EntityManager.RemoveComponent<StoreData_Spawner>(entity);

        }).Run();
    }
}