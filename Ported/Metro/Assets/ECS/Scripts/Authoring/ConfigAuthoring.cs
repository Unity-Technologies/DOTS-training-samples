using Unity.Entities;
using UnityEngine;

class ConfigAuthoring : MonoBehaviour
{
    public GameObject TrainPrefab;
    public int TrainsToSpawn;
}

class ConfigBaker : Baker<ConfigAuthoring>
{
    public override void Bake(ConfigAuthoring authoring)
    {
        AddComponent(new Config
        {
            TrainPrefab = GetEntity(authoring.TrainPrefab),
            TrainsToSpawn = authoring.TrainsToSpawn,
        });
    }
}