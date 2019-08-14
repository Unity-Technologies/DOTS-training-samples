
using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using static TrackSplineSpawner_TEST;

// ReSharper disable once InconsistentNaming
[RequiresEntityConversion]
public class CarSpeedAuthoring_TEST : MonoBehaviour, IConvertGameObjectToEntity
{
    public float speed;
    public int currentTrackSpline;


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new MovementSpeedComponent_TEST { speed = speed* UnityEngine.Random.Range(1,2), currentTrackSpline = currentTrackSpline, dir = 1 });
    }
}
