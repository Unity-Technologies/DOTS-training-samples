using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
public class WaterAuthoring : MonoBehaviour
    ,IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager
        , GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<River>(entity);
        dstManager.AddComponentData(entity, new URPMaterialPropertyBaseColor()
        {
            Value = new float4(0.0f, 0.0f, 1.0f, 1.0f)
        });
        dstManager.AddComponentData(entity, new Volume()
        {
            Value = 1.0f
        });  
    }
}