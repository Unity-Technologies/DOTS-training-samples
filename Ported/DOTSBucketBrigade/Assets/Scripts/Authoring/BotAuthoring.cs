using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class BotAuthoring : MonoBehaviour
    , IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager
        , GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new URPMaterialPropertyBaseColor()
        {
            Value = new float4(1.0f, 1.0f, 1.0f, 1.0f)
        });

        dstManager.AddComponentData(entity, new Speed()
        {
            Value = new float3(0.2f, 0.0f, 0.2f)
        });

        dstManager.AddComponentData(entity, new TargetPosition()
        {
            Value = new float3(0.0f, 0.0f, 0.0f)
        });

        dstManager.AddComponent<NextPerson>(entity);
        dstManager.AddComponent<BucketID>(entity);
        dstManager.AddComponentData(entity, new Radius()
        {
            Value = 5
        });
    }
}