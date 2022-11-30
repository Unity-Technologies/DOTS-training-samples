using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class PlatformIdAuthoring : MonoBehaviour
{
    //public int Value;
}
class PlatformIdBaker : Baker<PlatformIdAuthoring>
{
    public override void Bake(PlatformIdAuthoring authoring)
    {
        AddComponent(new PlatformId{ Value = 0 });
    }
}