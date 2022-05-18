using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class PlatformAuthoring : MonoBehaviour
{
}


public class PlatformBaker : Baker<PlatformAuthoring>
{
    public override void Bake(PlatformAuthoring authoring)
    {
        var buffer = AddBuffer<ChildrenWithRenderer>().Reinterpret<Entity>();
        foreach (var renderer in GetComponentsInChildren<UnityEngine.MeshRenderer>())
        {
            buffer.Add(GetEntity(renderer));
        }
    }
}
