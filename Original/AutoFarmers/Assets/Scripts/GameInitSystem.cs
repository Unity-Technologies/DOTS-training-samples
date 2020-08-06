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
                    var tile = EntityManager.Instantiate(groundData.debugOptions == MapDebugOptions.Tilled ? groundData.tilledGroundEntity : groundData.defaultGroundEntity);
                    EntityManager.RemoveComponent<Translation>(tile);
                    EntityManager.RemoveComponent<Rotation>(tile);
                    EntityManager.RemoveComponent<Scale>(tile);
                    EntityManager.RemoveComponent<NonUniformScale>(tile);
                    EntityManager.AddComponentData<Position2D>(tile,new Position2D { position = new Unity.Mathematics.float2(x,y) });

                    var renderer = EntityManager.GetSharedComponentData<RenderMesh>(tile);

                    switch (groundData.debugOptions)
                    {
                        case MapDebugOptions.Empty:
                            // EntityManager.AddComponent<PlantedSeedTag>(tile);
                            continue;
                        case MapDebugOptions.Tilled:
                            EntityManager.AddComponent<GroundTilledState>(tile);
                            break;
                    }
                }
            }
            EntityManager.RemoveComponent<GroundData>(entity);
        }).Run();

    }
}