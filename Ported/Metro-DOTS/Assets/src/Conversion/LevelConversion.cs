using Unity.Entities;
using UnityEngine;

public class LevelConversion : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        // EntityManager -> In CONVERSION world: will not be saved, stored or serialized
        // DstEntityManager -> In destination world: what will be serialized into the subscene

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

        metroComponent.SetupMetroLines();
        //Pathfinding.GeneratePathFindingData(Pathfinding.GetAllPlatformsInScene());
        var lookup = PathLookupHelper.CreatePathLookup(Pathfinding.GetAllPlatformsInScene());
    }

    void GenerateTrainTracksBezierData(Entity entity, Metro metroComponent)
    {
        Debug.Log("GenerateTrainTracksBezierData");
    }
}
