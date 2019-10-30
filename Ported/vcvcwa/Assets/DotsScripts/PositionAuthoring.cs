using System;
using System.Collections.Generic;
using System.Numerics;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[RequiresEntityConversion]
[AddComponentMenu("DOTS Samples/Positions/Position")]
public class PositionAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var data = new PositionComponent()
        {
            position = transform.position
        };
        
        dstManager.AddComponentData(entity, data);
    }
}
