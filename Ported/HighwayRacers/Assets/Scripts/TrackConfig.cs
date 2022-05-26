using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct TrackConfig : IComponentData
{
    public int numberOfCars;
    public int highwaySize;
    public float switchLanesSpeed;
    public bool dirty;
}

public struct TrackConfigMinMax : IComponentData
{
    public int minNumberOfCars;
    public int maxNumberOfCars;
    public int minHighwaySize;
    public int maxHighwaySize;
}