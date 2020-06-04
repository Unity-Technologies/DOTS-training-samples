using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[RequiresEntityConversion]
public class RoadCreationAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float LaneWidth;
    public int MaxLanes;

    //Move this probably
    public float CarSpawningDistance;
    
    public float MidRadius;
    public float StraightPieceLength;

    public int SegmentCount;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new RoadInfo()
        {
            LaneWidth = LaneWidth,
            MaxLanes = MaxLanes,
            CarSpawningDistancePercent = CarSpawningDistance,
            MidRadius = MidRadius,
            StraightPieceLength = StraightPieceLength,
            SegmentCount = SegmentCount
        });

        DynamicBuffer<LaneInfoElement> dynamicBuffer = dstManager.AddBuffer<LaneInfoElement>(entity);
        for (int i = 0; i < MaxLanes; ++i)
        {
            LaneInfo info = new LaneInfo
            {
                Pivot = LaneWidth * ((MaxLanes - 1) / 2f - i),
                Radius = MidRadius - LaneWidth * (MaxLanes - 1) / 2f + i * LaneWidth
            };

            info.CurvedPieceLength = info.Radius * math.PI / 2f;
            
            dynamicBuffer.Add(new LaneInfoElement { Value = info });
        }
    }
}
