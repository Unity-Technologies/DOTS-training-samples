using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class RoadCreationAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float2 StartXZ;
    public float2 EndXZ;
    public int MaxLanes;

    //Move this probably
    public float CarSpawningDistance;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        float laneWidth = math.abs(EndXZ.x - StartXZ.x) / MaxLanes;
        dstManager.AddComponentData(entity, new RoadInfo()
        {
            StartXZ = StartXZ,
            EndXZ = EndXZ,
            LaneWidth = laneWidth,
            MaxLanes = MaxLanes,
            CarSpawningDistance = CarSpawningDistance,
        });

        DynamicBuffer<LaneInfoElement> dynamicBuffer = dstManager.AddBuffer<LaneInfoElement>(entity);
        float currentLanePos = StartXZ.x + laneWidth / 2;
        for (int i = 0; i < MaxLanes; ++i)
        {
            LaneInfo info = new LaneInfo { Pivot = currentLanePos };
            dynamicBuffer.Add(new LaneInfoElement { Value = info });
            currentLanePos += laneWidth;
        }
    }
}
