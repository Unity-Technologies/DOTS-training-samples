using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct Road : IComponentData
{
}

public class RoadAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public List<GameObject> nodeList;
    
    public void Convert(Entity entity, EntityManager dstManager,
        GameObjectConversionSystem conversionSystem)
    {
    }
}