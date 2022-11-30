using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public class TrainAuthoring : MonoBehaviour
{
    public int carriageAmount = 5;
    public GameObject carriagePrefab;
}

class TrainAuthoringBaker : Baker<TrainAuthoring>
{
    public override void Bake(TrainAuthoring authoring)
    {
        AddComponent(new SpeedComponent
        {
            Current = 0.1f,
            Max = 1f
        });
        AddComponent(new MetroLineID
        {
            ID = 0
        });
        AddComponent<URPMaterialPropertyBaseColor>();
        AddComponent<Train>();
        AddComponent(new TrainSpawn
        {
            CarriageSpawn = GetEntity(authoring.carriagePrefab),
            CarriageCount = authoring.carriageAmount
        });
    }
}