using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class FetcherAuthoring : MonoBehaviour
    , IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager
        , GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<BucketFetcher>(entity);
        dstManager.AddComponentData(entity, new Speed()
        {
            Value = new float3(0.05f, 0.0f, 0.05f)
        });

        dstManager.AddComponentData(entity, new TargetPosition()
        {
            Value = new float3(0.0f, 0.0f, 0.0f)
        });

        dstManager.AddComponent<BucketID>(entity);
        dstManager.AddComponentData(entity, new URPMaterialPropertyBaseColor()
        {
            Value = new float4(1.0f, 0.0f, 1.0f, 1.0f)
        });
    }
}