using System.ComponentModel.Design.Serialization;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class BucketSpawnSystem : SystemBase
{
    private EntityQuery m_WaterBucketSpawnerQuery;
    private EntityQuery m_WaterBucketQuery;
    private EntityQuery m_FirefighterFullTagQuery;

    protected override void OnCreate()
    {
        RequireForUpdate(m_WaterBucketSpawnerQuery);
        
        m_WaterBucketQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[]
            {
                typeof(WaterBucketTag),
            }
        });
        
        m_FirefighterFullTagQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[]
            {
                typeof(FirefighterFullTag),
            }
        });
    }
    
    protected override void OnUpdate()
    {
        Debug.Log("BucketSpawnSystem 0");
        
        Entity firstFirefighter = Entity.Null;
        using (var firefighterEntities = m_FirefighterFullTagQuery.ToEntityArray(Allocator.TempJob))
        {
            if (firefighterEntities.Length == 0)
                return;

            firstFirefighter = firefighterEntities[0];
        }

        Entities.WithStructuralChanges()
            .WithStoreEntityQueryInField(ref m_WaterBucketSpawnerQuery)
            .ForEach((Entity entity, in WaterBucketSpawner spawner, in LocalToWorld ltw) =>
            {
                for (int i = 0; i < spawner.Count; ++i)
                {
                    var instance = EntityManager.Instantiate(spawner.Prefab);
                    SetComponent<Translation2D>(instance, new Translation2D { Value = 0 });
                    EntityManager.AddComponent<WaterBucketTag>(instance);
                }

                EntityManager.DestroyEntity(entity);
            }).Run();

        Debug.Log("BucketSpawnSystem 1");
        
        Entity firstWaterBucket = Entity.Null;
        using (var waterBucketEntities = m_WaterBucketQuery.ToEntityArray(Allocator.TempJob))
        {
            if (waterBucketEntities.Length == 0)
                return;
        
            firstWaterBucket = waterBucketEntities[0];
        
            //EntityManager.AddComponentData(firstFirefighter, new WaterBucketID { Value = firstWaterBucket });
        }

        Entities
            .WithStructuralChanges()
            .WithAll<FirefighterFullTag>()
            .ForEach((int entityInQueryIndex, Entity entity) =>
            {
                if (entityInQueryIndex == 0)
                {
                    EntityManager.AddComponent<WaterBucketID>(entity);
                    SetComponent(entity, new WaterBucketID { Value = firstWaterBucket });
                }
            }).Run();
        
        Debug.Log("BucketSpawnSystem 2");
    }
}
