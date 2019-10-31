using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public struct TornadoData : IComponentData
{
    public float spinRate;
    public float upwardSpeed;
    public float radiusMultiplier;
    public float tornadoSway;
    public float tornadoX;
    public float tornadoZ;
}

[RequiresEntityConversion]
public class TornadoEmitterComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    public float spinRate;
    public float upwardSpeed;
    public float radiusMultiplier;
    public float tornadoSway;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) 
    {
        var data = new TornadoData
        {
            spinRate = spinRate,
            upwardSpeed = upwardSpeed,
            radiusMultiplier = radiusMultiplier,
            tornadoSway = tornadoSway
        };
        dstManager.AddComponentData(entity, data);
        dstManager.RemoveComponent<LocalToWorld>(entity);
        dstManager.RemoveComponent<Translation>(entity);
        dstManager.RemoveComponent<Rotation>(entity);
    }
}