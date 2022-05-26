using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class TrackConfigAuthoring : MonoBehaviour
{
    public int defaultNumberOfCars = 50;
    public int defaultHighwaySize = 500;
    public int minNumberOfCars = 0;
    public int maxNumberOfCars = 500;
    public int minHighwaySize = 115;
    public int maxHighwaySize = 500;
    public float switchLanesSpeed = 0.5f;
}

public class TrackConfigBaker : Baker<TrackConfigAuthoring>
{
    public override void Bake(TrackConfigAuthoring authoring)
    {
        AddComponent(new TrackConfig
        {
            numberOfCars = authoring.defaultNumberOfCars,
            highwaySize = authoring.defaultHighwaySize,
            switchLanesSpeed = authoring.switchLanesSpeed
        });

        AddComponent(new TrackConfigMinMax
        {
            minNumberOfCars = authoring.minNumberOfCars,
            maxNumberOfCars = authoring.maxNumberOfCars,
            minHighwaySize = authoring.minHighwaySize,
            maxHighwaySize = authoring.maxHighwaySize
        });
    }
}