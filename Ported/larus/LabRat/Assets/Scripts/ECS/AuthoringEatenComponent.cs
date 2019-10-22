using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct EatenComponentTag : IComponentData
{
};

public class AuthoringEatenComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<EatenComponentTag>(entity);
    }
}
