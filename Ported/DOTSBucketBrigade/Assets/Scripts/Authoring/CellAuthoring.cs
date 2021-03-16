using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
public class CellAuthoring : MonoBehaviour
    ,IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager
        , GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData<Heat>(entity, new Heat() 
        {
        });
        dstManager.AddComponentData(entity, new URPMaterialPropertyBaseColor()
        {
            Value = new float4(0.0f, 1.0f, 0.0f, 1.0f)
        });
        dstManager.AddComponentData(entity, new NonUniformScale()
        {
            Value = new float3(1.0f, 0.1f, 1.0f)
        });    
    }
}
