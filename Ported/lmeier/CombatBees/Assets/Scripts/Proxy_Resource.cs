using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;

[RequireComponent(typeof(ConvertToEntity))]
public class Proxy_Resource : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        C_Velocity vel = new C_Velocity() { Value = Vector3.zero };

        dstManager.AddComponent(entity, typeof(Tag_Resource));
        dstManager.AddComponentData(entity, vel);
        dstManager.AddComponent(entity, typeof(C_GridIndex));
        dstManager.AddComponent(entity, typeof(C_Held));
    }
}
