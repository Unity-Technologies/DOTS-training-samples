using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Serialization;

[Serializable]
public struct TrainSpawner : IComponentData
{
    public Entity LeaderPrefab;
    public int NoOfTrains;
    public Entity FollowerPrefab;
    public int NoOfCartPerTrain;
}
