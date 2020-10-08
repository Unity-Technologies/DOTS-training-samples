using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class OnConvertToEntity : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        Transform t = this.transform;
        
        dstManager.RemoveComponent<Translation>(entity);
        dstManager.RemoveComponent<Rotation>(entity);
        if (dstManager.HasComponent<NonUniformScale>(entity))
            dstManager.RemoveComponent<NonUniformScale>(entity);

        if (!dstManager.HasComponent<MyScale>(entity))
        {
            if (dstManager.HasComponent<MyNonUniformScale>(entity))
            {
                dstManager.SetComponentData(entity, new MyNonUniformScale
                {
                    Value = new float3(t.localScale.x, t.localScale.y, t.localScale.z)
                });
            }
            else
            {
                dstManager.AddComponentData(entity, new MyNonUniformScale
                {
                    Value = new float3(t.localScale.x, t.localScale.y, t.localScale.z)
                });
            }
        }

        if (dstManager.HasComponent<Pos>(entity))
        {
            dstManager.SetComponentData(entity, new Pos
            {
                Value = new float2(t.position.x, t.position.z)
            });
        }
        else
        {
            dstManager.AddComponentData(entity, new Pos
            {
                Value = new float2(t.position.x, t.position.z)
            });
        }
    }
}
