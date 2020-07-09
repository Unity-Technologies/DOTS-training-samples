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
                    EntityManager.AddComponent<WaterBucketTag>(instance);
                }

                EntityManager.DestroyEntity(entity);
            }).Run();

        Entity firstWaterBucket = Entity.Null;
        using (var waterBucketEntities = m_WaterBucketQuery.ToEntityArray(Allocator.TempJob))
        {
            if (waterBucketEntities.Length == 0)
                return;
        
            firstWaterBucket = waterBucketEntities[0];
        
            EntityManager.AddComponentData(firstFirefighter, new WaterBucketID { Value = firstWaterBucket });
        }
    }
}
