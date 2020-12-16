using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine.UIElements;


public class ObstacleAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<Translation>(entity);
        dstManager.AddComponent<Obstacle>(entity);
        dstManager.AddComponent<Radius>(entity);
        dstManager.RemoveComponent<Rotation>(entity);
    }
}

