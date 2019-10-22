using System.Collections;
using Unity.Entities;
using UnityEngine;

public class LevelConversion : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach<Metro>(Convert);
    }

    void Convert(Entity entity, Metro metroComponent)
    {
        Debug.Log("Start Metro Conversion");
        GeneratePathfindingData(entity, metroComponent);
        GenerateTrainTracksBezierData(entity, metroComponent);
    }

    void GeneratePathfindingData(Entity entity, Metro metroComponent)
    {
        Debug.Log("GeneratePathfindingData");
    }

    void GenerateTrainTracksBezierData(Entity entity, Metro metroComponent)
    {
        Debug.Log("GenerateTrainTracksBezierData");
    }
}
