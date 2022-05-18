using Unity.Entities;
using UnityEngine;

public class PlatformAuthoring : MonoBehaviour
{
}

public class PlatformBaker : Baker<PlatformAuthoring>
{
    public override void Bake(PlatformAuthoring authoring)
    {
       AddComponent(new Platform());
    }
}