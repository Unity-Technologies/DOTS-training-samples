using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class AuthoringEaterComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent(entity, typeof(EaterComponentTag));
        dstManager.AddComponent(entity, typeof(LastPositionComponent));
    }
}
