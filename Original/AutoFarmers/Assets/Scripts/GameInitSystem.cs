using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class GameInitSystem:SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithStructuralChanges().ForEach((Entity entity,in GroundData groundData) =>
        {
            for(int y = 0;y < groundData.fieldSizeY;++y)
            {
                for(int x = 0;x < groundData.fieldSizeX;++x)
                {
                    var tile = EntityManager.Instantiate(groundData.groundEntity);
                    EntityManager.RemoveComponent<Translation>(tile);
                    EntityManager.RemoveComponent<Rotation>(tile);
                    EntityManager.RemoveComponent<Scale>(tile);
                    EntityManager.RemoveComponent<NonUniformScale>(tile);
                    EntityManager.AddComponentData<Position2D>(tile,new Position2D { position = new Unity.Mathematics.float2(x,y) });

                    var renderer = EntityManager.GetSharedComponentData<RenderMesh>(tile);

                    switch(Farm.instance.debugOptions)
                    {
                        case Farm.MapDebugOptions.Empty:
                            break;
                        case Farm.MapDebugOptions.Tilled:
                            EntityManager.AddComponent<GroundTilledState>(tile);
                            break;
                    }
                }
            }
            EntityManager.RemoveComponent<GroundData>(entity);
        }).Run();


        Entities.WithStructuralChanges().ForEach((Entity entity,in FarmerData farmerData) =>
        {
            var farmer = EntityManager.Instantiate(farmerData.farmerEntity);
            EntityManager.RemoveComponent<Translation>(farmer);
            EntityManager.RemoveComponent<Rotation>(farmer);
            EntityManager.RemoveComponent<Scale>(farmer);
            EntityManager.RemoveComponent<NonUniformScale>(farmer);
            EntityManager.AddComponentData<Position2D>(farmer,new Position2D { position = new Unity.Mathematics.float2(0,0) });
            int posX = UnityEngine.Random.Range(0,10);
            int posY = UnityEngine.Random.Range(0,10);
            EntityManager.AddComponentData<WorkerTeleport>(farmer, new WorkerTeleport { position = new Unity.Mathematics.float2(posX,posY) });

            EntityManager.RemoveComponent<FarmerData>(entity);

        }).Run();

        // Spawn rocks
        Entities.WithStructuralChanges().ForEach((Entity entity, in RockAuthoring rockData) =>
        {
            for(int i = 0; i < 20; i++)
            {
                int rockX = Random.Range(0, rockData.mapX);
                int rockY = Random.Range(0, rockData.mapY);

                var farmer = EntityManager.Instantiate(rockData.rockEntity);
                EntityManager.RemoveComponent<Translation>(farmer);
                EntityManager.RemoveComponent<Rotation>(farmer);
                EntityManager.RemoveComponent<Scale>(farmer);
                EntityManager.RemoveComponent<NonUniformScale>(farmer);
                EntityManager.AddComponentData<Position2D>(farmer, new Position2D { position = new Unity.Mathematics.float2(rockX, rockY) });
            }
            

            EntityManager.RemoveComponent<RockAuthoring>(entity);

        }).Run();
    }
}