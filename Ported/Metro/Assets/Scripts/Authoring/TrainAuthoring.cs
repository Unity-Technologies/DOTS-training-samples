using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class TrainAuthoring : MonoBehaviour
{
    public int carriageAmount = 5;
    public GameObject carriagePrefab;
    public float MaxSpeed = 1;
}

class TrainAuthoringBaker : Baker<TrainAuthoring>
{
    public override void Bake(TrainAuthoring authoring)
    {
        AddComponent(new SpeedComponent
        {
            Current = 0.1f * authoring.MaxSpeed,
            Max = authoring.MaxSpeed
        });
        AddComponent(new MetroLineID
        {
            ID = 0
        });
        AddComponent<URPMaterialPropertyBaseColor>();
        AddComponent<Train>();
        AddComponent<TrainStateComponent>();
        AddComponent<UniqueTrainID>();
        AddComponent<TrainIndexOnMetroLine>();
    }
}