using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public struct Color : IComponentData
{
    public float4 Value;
}

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ColorComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var data = new Color { Value = float4.zero };
        dstManager.AddComponentData(entity, data);
    }
}