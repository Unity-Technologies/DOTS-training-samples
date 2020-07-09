using System.ComponentModel.Design.Serialization;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class BucketSpawnSystem : SystemBase
{
    private EntityQuery m_WaterBucketSpawnerQuery;

    protected override void OnCreate()
    {
        RequireForUpdate(m_WaterBucketSpawnerQuery);
    }
    
    protected override void OnUpdate()
    {
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

        Entities
            .WithStructuralChanges()
            .WithAll<FirefighterFullTag>()
            .ForEach((int entityInQueryIndex, Entity entity) =>
            {
                if (entityInQueryIndex == 0)
                    EntityManager.AddComponent<WaterBucketID>(entity);
            }).Run();
    }
}
