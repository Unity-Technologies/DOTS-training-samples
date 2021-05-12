using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct TrainSpawnerComponent : IComponentData
{
    public Entity TrainEnginePrefab;
    public Entity TrainCarPrefab;

    public int numberOfTrainsPerTrack;
    public int numberOfTrainCarsPerTrain;
    public int numberOfTracks;
}
