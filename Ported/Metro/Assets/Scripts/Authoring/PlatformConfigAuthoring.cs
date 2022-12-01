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

// TODO - Move this to another file
// [InternalBufferCapacity(40)]
// public struct Platform : IBufferElementData
// {
//     public Platform Value;
// }