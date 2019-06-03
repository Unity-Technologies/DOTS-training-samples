using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;

[RequireComponent(typeof(ConvertToEntity))]
public class Proxy_Particle : MonoBehaviour, IConvertGameObjectToEntity
{

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        C_Velocity vel = new C_Velocity() { Value = Vector3.zero };

        dstManager.AddComponent(entity, typeof(Tag_Particle));
        dstManager.AddComponent(entity, typeof(Tag_Particle_Init));
        dstManager.AddComponent(entity, typeof(Tag_IsDying));
        dstManager.AddComponent(entity, typeof(C_PreviousPos));
        dstManager.AddComponent(entity, typeof(C_DeathTimer));
        dstManager.AddComponent(entity, typeof(C_Size));
        dstManager.AddComponentData(entity, vel);
        dstManager.AddComponent(entity, typeof(Tag_ILookWhereImGoing));
        dstManager.AddComponent(entity, typeof(Tag_StretchByVelocity));
        dstManager.AddComponent(entity, typeof(Tag_Sticky));
    }
}