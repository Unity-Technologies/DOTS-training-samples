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
    public Vector3 gravity = new Vector3(0f, -9.81f, 0f);
    public Renderer fieldRenderer;
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
            maximumBeeSize = authoring.maximumBeeSize,
            gravity = authoring.gravity,
            fieldSize = authoring.fieldRenderer.bounds.extents * 2f
        });
    }
}