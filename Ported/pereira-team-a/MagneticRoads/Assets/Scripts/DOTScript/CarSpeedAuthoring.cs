using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

// ReSharper disable once InconsistentNaming
[RequiresEntityConversion]
public class CarSpeedAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float speed;
    public float timer;
    public float3 startPoint;
    public float3 endPoint;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new MovementSpeedComponent { speed = speed });
        dstManager.AddComponentData(entity, new InterpolatorTComponent { t = 0.1f });
        dstManager.AddComponentData(entity, new TrackSplineComponent { startPoint = startPoint,endPoint = endPoint, startNormal = math.up(),endNormal = new float3(1,1,0) });
    }
}