using System;
using Unity.Entities;
using UnityEngine;

public enum DistanceFieldModel
{
    SpherePlane,
    Metaballs,
    SpinMixer,
    SphereField,
    FigureEight,
    PerlinNoise,
}

[Serializable]
public struct DistanceField : IComponentData
{
    public DistanceFieldModel model;
    public float switchTimer; // how long you want to stay on a particular distance field model
    public float switchTimerValue; // countdown to switch
}

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class DistanceFieldComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    public DistanceFieldModel model;
    public float switchTimerLength;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var data = new DistanceField { model = model, switchTimer = switchTimerLength, switchTimerValue = switchTimerLength };
        dstManager.AddComponentData(entity, data);
    }
}