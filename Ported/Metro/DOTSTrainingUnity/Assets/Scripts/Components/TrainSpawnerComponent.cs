using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct TrainSpawnerComponent : IComponentData
{
    public Entity TrainEnginePrefab;
    public Entity TrainCarPrefab;

    public int numberOfTrainsPerTrack;
    public int numberOfTrainCarsPerTrain;
    public int numberOfTracks;

    public float4 color0;
    public float4 color1;
    public float4 color2;
    public float4 color3;
}
