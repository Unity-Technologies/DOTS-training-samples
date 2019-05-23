using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public struct Velocity : IComponentData
{
    public float3 Value;
}

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class VelocityComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var data = new Velocity { Value = float3.zero };
        dstManager.AddComponentData(entity, data);
    }
}