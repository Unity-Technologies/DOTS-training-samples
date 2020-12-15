using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class AntAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<Heading>(entity);
        dstManager.AddComponent<Ant>(entity);
    }
}
