using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

[GenerateAuthoringComponent]
public struct FireSim : IComponentData
{
    public int ChainCount;
    public int NumBotsPerChain;

    public int BucketCount;
    public int WaterCellCount;

    public int FireGridDimension;
    public int PropagationRadius;
    [Range(0.0f, 1.0f)] public float FlashPoint;
    [Range(0.0f, 1.0f)] public float IgnitionRate;
    [Range(0.0f, 1.0f)] public float HeatTransfer;

    public static Entity GetClosestEntity(float3 position, NativeArray<Entity> emptyEntities, NativeArray<Translation> translations)
    {
        float distance = float.MaxValue;
        Entity closestEntity = Entity.Null;
        for (int i = 0; i < emptyEntities.Length; i++)
        {
            var entity = emptyEntities[i];
            var translation = translations[i].Value;
            var currentDistance = math.distance(position, translation);

            if (currentDistance < distance)
            {
                distance = currentDistance;
                closestEntity = entity;
            }
        }

        return closestEntity;
    }
}
