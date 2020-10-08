using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(FarmerInitSystem))]
public class FakeCreateCropCarrier : SystemBase
{
    EntityQuery m_FarmerQuery;
    
    protected override void OnCreate()
    {
        m_FarmerQuery = GetEntityQuery(typeof(Farmer), ComponentType.Exclude<DropOffCropTask>());
    }
    
    protected override void OnUpdate()
    {
        var gameState = GetSingletonEntity<GameState>();
        var prefab = GetComponent<GameState>(gameState).PlantPrefab;

        int count = m_FarmerQuery.CalculateEntityCount();
        NativeArray<Entity> farmers = m_FarmerQuery.ToEntityArray(Allocator.TempJob);
        
        for(int i = 0; i < count; i++)
        {
            Entity farmerEntity = farmers[i];
            Position farmerPos = GetComponent<Position>(farmerEntity);

            float4 color = new float4(1.0f, 0.75f, 0.0f, 1.0f);

            float s = 1f / 1.5f;
            float3 scale = new float3(s, s, s);

            var cropEntity = EntityManager.Instantiate(prefab);
            EntityManager.AddComponent<ECSMaterialOverride>(cropEntity);
            EntityManager.SetComponentData(cropEntity, new ECSMaterialOverride { Value = color });
            EntityManager.SetComponentData(cropEntity,
                new Translation { Value = new float3(farmerPos.Value.x, 0.5f, farmerPos.Value.y) });
            EntityManager.AddComponent<NonUniformScale>(cropEntity);
            EntityManager.SetComponentData(cropEntity, new NonUniformScale { Value = scale });
            EntityManager.AddComponent<Crop>(cropEntity);

            EntityManager.AddComponent<CropCarried>(cropEntity);
            EntityManager.SetComponentData(cropEntity, new CropCarried { FarmerOwner = farmerEntity });

            EntityManager.AddComponent<DropOffCropTask>(farmerEntity);
            
            NativeArray<Entity> bestDepotEntity = new NativeArray<Entity>(1, Allocator.TempJob);
            float minDistToDepot = float.MaxValue;
            
            Entities
                .WithAll<Depot>()
                .ForEach((Entity entity, in Position position) =>
                {
                    float dist = math.distancesq(farmerPos.Value, position.Value); 
                    if( dist > 1f && dist < minDistToDepot)
                    {
                        minDistToDepot = dist;
                        bestDepotEntity[0] = entity;
                    }
                }).Run();
            
            EntityManager.AddComponentData(farmerEntity, new TargetEntity(){target = bestDepotEntity[0], targetPosition = GetComponent<Position>(bestDepotEntity[0]).Value});
            EntityManager.AddComponentData(cropEntity, new TargetEntity(){target = bestDepotEntity[0], targetPosition = GetComponent<Position>(bestDepotEntity[0]).Value});
            
            bestDepotEntity.Dispose();
        }

        farmers.Dispose();
    }
}
