using Unity.Entities;
using UnityEngine;

class ConfigAuthoring : MonoBehaviour
{
    public GameObject beePrefab;
    public int startBeeCount;
    public float minimumBeeSize;
    public float maximumBeeSize;
}

class ConfigBaker : Baker<ConfigAuthoring>
{
    public override void Bake(ConfigAuthoring authoring)
    {
        AddComponent(new Config()
        {
            beePrefab = GetEntity(authoring.beePrefab),
            startBeeCount = authoring.startBeeCount,
            minimumBeeSize = authoring.minimumBeeSize,
            maximumBeeSize = authoring.maximumBeeSize
        });
    }
}