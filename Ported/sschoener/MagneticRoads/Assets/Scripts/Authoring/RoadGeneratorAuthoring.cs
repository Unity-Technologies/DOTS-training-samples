using System;
using Unity.Entities;
using UnityEngine;

public class RoadGeneratorAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public RoadGeneratorDots RoadGenerator;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        RoadGenerator.SpawnRoads();
        dstManager.AddComponentData(entity, new SplineSetupComponent
        {
            Splines = TrackSplinesBlob.Instance,
            Intersections = IntersectionsBlob.Instance
        });
    }
}