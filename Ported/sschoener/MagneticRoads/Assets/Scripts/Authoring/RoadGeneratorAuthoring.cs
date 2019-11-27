using System;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class RoadGeneratorAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public RoadGeneratorDots RoadGenerator;

    public Mesh IntersectionMesh;
    public Material RoadMaterial;
    public float TrackRadius = .2f;
    public float TrackThickness = .05f;
    public float IntersectionSize = .5f;
    public int TrisPerMesh = 4000;
    public int SplineResolution = 20;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        RoadGenerator.SpawnRoads();
        dstManager.AddComponentData(entity, new RoadSetupComponent
        {
            Splines = TrackSplinesBlob.Instance,
            Intersections = IntersectionsBlob.Instance,
            IntersectionSize = IntersectionSize,
            TrackRadius = TrackRadius,
            TrackThickness = TrackThickness,
            TrisPerMesh = TrisPerMesh,
            SplineResolution = SplineResolution,
        });
        dstManager.AddSharedComponentData(entity, new RenderMesh
        {
            mesh = IntersectionMesh,
            material = RoadMaterial,
        });
    } 
}