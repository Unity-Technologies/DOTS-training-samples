using Unity.Entities;
using UnityEngine;

public class TrainConfigAuthoring : MonoBehaviour
{
    public GameObject TrainPrefab;
    public GameObject CarriagePrefab;
    public int CarriageCount;
}

class TrainConfigBaker : Baker<TrainConfigAuthoring>
{
    public override void Bake(TrainConfigAuthoring authoring)
    {
        AddComponent(new TrainConfig
        {
            TrainPrefab = GetEntity(authoring.TrainPrefab),
            CarriagePrefab = GetEntity(authoring.CarriagePrefab),
            CarriageCount = authoring.CarriageCount
        });
    }
}