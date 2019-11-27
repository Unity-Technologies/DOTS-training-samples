using System;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class RoadGeneratorAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public RoadGeneratorDots RoadGenerator;

    public Mesh IntersectionMesh;
    public Material RoadMaterial;
    public float TrackThickness = .05f;
    public float IntersectionSize = .5f;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        RoadGenerator.SpawnRoads();
        dstManager.AddComponentData(entity, new RoadSetupComponent
        {
            Splines = TrackSplinesBlob.Instance,
            Intersections = IntersectionsBlob.Instance,
            IntersectionSize = IntersectionSize,
            TrackThickness = TrackThickness
        });
        dstManager.AddSharedComponentData(entity, new RenderMesh
        {
            mesh = IntersectionMesh,
            material = RoadMaterial,
        });
    } 
}