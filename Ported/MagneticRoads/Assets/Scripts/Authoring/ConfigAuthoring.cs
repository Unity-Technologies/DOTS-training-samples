using Unity.Entities;
using UnityEngine;

#region step1
class ConfigAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject CarPrefab;
    public int CarCount;
    public float BrakingDistanceThreshold = 0.5f;
    
    public int voxelCount=60;
    public float voxelSize = 1f;
    public int trisPerMesh = 4000;
    public Material roadMaterial;
    public Mesh intersectionMesh;
    public Mesh intersectionPreviewMesh;
    public float carSpeed=2f;
    
}

class ConfigBaker : Baker<ConfigAuthoring>
{
    public override void Bake(ConfigAuthoring authoring)
    {
        AddComponent(new Config
        {
            CarPrefab = GetEntity(authoring.CarPrefab),
            CarCount = authoring.CarCount,
            BrakingDistanceThreshold = authoring.BrakingDistanceThreshold
        });
    }
}
#endregion