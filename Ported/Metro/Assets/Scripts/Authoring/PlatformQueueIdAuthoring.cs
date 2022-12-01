using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class PlatformQueueIdAuthoring : MonoBehaviour
{
    public int Id;
}
class PlatformQueueIdBaker : Baker<PlatformQueueIdAuthoring>
{
    public override void Bake(PlatformQueueIdAuthoring authoring)
    {
        AddComponent(new PlatformQueueId{ Value = authoring.Id });
    }
}