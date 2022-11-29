using Unity.Entities;
using UnityEngine;

public class PlatformConfigAuthoring : MonoBehaviour
{
    public GameObject PlatformPrefab;
}

class PlatformConfigBaker : Baker<PlatformConfigAuthoring>
{
    public override void Bake(PlatformConfigAuthoring authoring)
    {
        AddComponent(new PlatformConfig
        {
            PlatformPrefab = GetEntity(authoring.PlatformPrefab)
        });
    }
}