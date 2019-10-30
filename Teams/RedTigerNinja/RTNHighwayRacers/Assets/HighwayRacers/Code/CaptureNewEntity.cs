using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class CaptureNewEntity : MonoBehaviour, Unity.Entities.IConvertGameObjectToEntity
{
    public static Entity CreatedEntity = Entity.Null;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        Debug.Log("Entity captured!");
        CreatedEntity = entity;
    }


}
