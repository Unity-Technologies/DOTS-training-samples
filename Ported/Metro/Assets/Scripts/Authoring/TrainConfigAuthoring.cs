using Unity.Entities;
using UnityEngine;

public class TrainConfigAuthoring : MonoBehaviour
{
    public GameObject TrainPrefab;
}

class TrainConfigBaker : Baker<TrainConfigAuthoring>
{
    public override void Bake(TrainConfigAuthoring authoring)
    {
        AddComponent(new TrainConfig
        {
            TrainPrefab = GetEntity(authoring.TrainPrefab)
        });
    }
}