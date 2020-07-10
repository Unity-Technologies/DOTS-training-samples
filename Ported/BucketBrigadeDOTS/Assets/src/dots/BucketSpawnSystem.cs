using System.ComponentModel.Design.Serialization;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(FirefighterFormLineSystem))]
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
                typeof(WaterBucket),
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
        Entity firstFirefighter = Entity.Null;
        using (var firefighterEntities = m_FirefighterFullTagQuery.ToEntityArray(Allocator.TempJob))
        {
            if (firefighterEntities.Length == 0)
                return;

            firstFirefighter = firefighterEntities[firefighterEntities.Length - 1];
        }

        Entities.WithStructuralChanges()
            .WithStoreEntityQueryInField(ref m_WaterBucketSpawnerQuery)
            .ForEach((Entity entity, in WaterBucketSpawner spawner, in LocalToWorld ltw) =>
            {
                for (int i = 0; i < spawner.Count; ++i)
                {
                    var instance = EntityManager.Instantiate(spawner.Prefab);
                    SetComponent<Translation2D>(instance, new Translation2D { Value = 0 });
                    EntityManager.AddComponentData<WaterBucket>(instance, new WaterBucket { Value = 1.0f });
                    EntityManager.AddComponentData(instance, new BaseColor() { Value = new float4(0.0f/255.0f, 250.0f/255.0f, 255.0f/255.0f, 1.0f) });
                }

                EntityManager.DestroyEntity(entity);
            }).Run();

        Entity firstWaterBucket = Entity.Null;
        using (var waterBucketEntities = m_WaterBucketQuery.ToEntityArray(Allocator.TempJob))
        {
            int waterBucketCount = waterBucketEntities.Length;
            if (waterBucketCount == 0)
                return;

            using (var firefighterEntities = m_FirefighterFullTagQuery.ToEntityArray(Allocator.TempJob))
            {
                int firefighterCount = firefighterEntities.Length;
                if (firefighterCount == 0)
                    return;

                int firefighterWithBucketStride = firefighterCount / waterBucketCount;

                for (int bucket = 0, firefighter = 0; bucket < waterBucketCount; bucket++, firefighter += firefighterWithBucketStride)
                {
                    EntityManager.AddComponentData(firefighterEntities[firefighter], new WaterBucketID { Value = waterBucketEntities[bucket] });
                }
            }
        }
    }
}
