using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class RoadCreationAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float2 StartXZ;
    public float2 EndXZ;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new LaneInfo()
        {
            StartXZ = StartXZ,
            EndXZ = EndXZ
        });
    }
}
