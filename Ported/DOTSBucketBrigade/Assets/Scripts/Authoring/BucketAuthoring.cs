using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
public class BucketAuthoring : MonoBehaviour
    ,IConvertGameObjectToEntity
{

    private static readonly float4 BUCKET_EMPTY_COLOUR = new float4(1.0f, 0.41037738f, 0.45895237f, 1.0f);

    public void Convert(Entity entity, EntityManager dstManager
        , GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<Bucket>(entity);
        dstManager.AddComponentData(entity, new URPMaterialPropertyBaseColor()
        {
            Value = BUCKET_EMPTY_COLOUR
        });

        dstManager.AddComponentData(entity, new Scale()
        {
            Value = 0.2f
        });

        dstManager.AddComponentData(entity, new Volume()
        {
            Value = 0.0f
        });  
    }
}


