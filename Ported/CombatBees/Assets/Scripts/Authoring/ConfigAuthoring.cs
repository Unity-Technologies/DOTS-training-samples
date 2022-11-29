using Unity.Entities;
using UnityEngine;

class ConfigAuthoring : MonoBehaviour
{
    public GameObject beePrefab;
    public GameObject particlePrefab;
    public int startBeeCount;
    public int beesPerResource;
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
            particlePrefab = GetEntity(authoring.particlePrefab),
            startBeeCount = authoring.startBeeCount,
            beesPerResource = authoring.beesPerResource,
            minimumBeeSize = authoring.minimumBeeSize,
            maximumBeeSize = authoring.maximumBeeSize
        });
    }
}