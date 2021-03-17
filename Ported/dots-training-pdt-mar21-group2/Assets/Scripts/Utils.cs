using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

struct Utils
{
    static public bool WorldIsOutOfBounds(Unity.Mathematics.float3 position, float width, float ground)
    {
        return position.x < 0.0f ||
                position.x > width ||
                position.y < ground;
    }
    
    static public bool FindNearestRock(
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