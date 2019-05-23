using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[Serializable]
[WriteGroup(typeof(LocalToWorld))]
public struct EmitterProperties : ISharedComponentData
{
    public float attraction;
    public float speedStretch;
    public float jitter;
    public UnityEngine.Color surfaceColor;
    public UnityEngine.Color interiorColor;
    public UnityEngine.Color exteriorColor;
    public float exteriorColorDist;
    public float interiorColorDist;
    public float colorStiffness;
}

[RequiresEntityConversion]
public class ParticleEmitterComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    //public Mesh particleMesh;
    //public Material particleMaterial;

    public float attraction;
    public float speedStretch;
    public float jitter;
    public UnityEngine.Color surfaceColor;
    public UnityEngine.Color interiorColor;
    public UnityEngine.Color exteriorColor;
    public float exteriorColorDist;
    public float interiorColorDist;
    public float colorStiffness;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var spawnerData = new EmitterProperties
        {
            attraction = attraction,
            speedStretch = speedStretch,
            jitter = jitter,
            surfaceColor = surfaceColor,
            interiorColor = interiorColor,
            exteriorColor = exteriorColor,
            exteriorColorDist = exteriorColorDist,
            interiorColorDist = interiorColorDist,
            colorStiffness = colorStiffness,
        };
        dstManager.AddSharedComponentData(entity, spawnerData);
    }
}
