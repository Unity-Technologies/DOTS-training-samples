using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class ProjectileSelectionSystem : SystemBase
{
    private EntityQuery m_AvailableRocksQuery;

    protected override void OnCreate()
    {
        m_AvailableRocksQuery = EntityManager.CreateEntityQuery(ComponentType.ReadOnly<Rock>(),
            ComponentType.ReadOnly<Available>());
    }
    
    protected override void OnUpdate()
    {
        var availableRocks = m_AvailableRocksQuery.ToEntityArray(Allocator.Temp);
        var translations = GetComponentDataFromEntity<Translation>();
        
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        Entities
            .WithAll<HandIdle>()
            .ForEach((Entity entity, ref TargetRock targetRock, in Translation translation) =>
            {
                FindNearestRock(translation, availableRocks, translations, out Entity nearestRock);
                targetRock.RockEntity = nearestRock;

                ecb.RemoveComponent<HandIdle>(entity);
                ecb.AddComponent(entity, new HandGrabbingRock());

            }).Run();

        availableRocks.Dispose();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }

    static bool FindNearestRock(
        Translation armTranslation,
        NativeArray<Entity> availableRocks, 
        ComponentDataFromEntity<Translation> translations, 
        out Entity nearestRock)
    {
        const float grabDist = 5.1f;
        const float grabDistSq = grabDist * grabDist;
        
        foreach(var rockEntity in availableRocks)
        {
            var rockTranslation = translations[rockEntity];
            var distSq = math.distancesq(armTranslation.Value, rockTranslation.Value);

            if (distSq < grabDistSq)
            {
                nearestRock = rockEntity;
                return true;
            }
        }

        nearestRock = default;
        return false;
    }
}