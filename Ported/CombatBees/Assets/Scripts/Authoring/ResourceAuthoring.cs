using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class ResourceAuthoring : UnityEngine.MonoBehaviour
{
}

class ResourceBaker : Baker<ResourceAuthoring>
{
    public override void Bake(ResourceAuthoring authoring)
    {
        var bounds = authoring.GetComponent<Renderer>().bounds;
        AddComponent<Resource>(new Resource()
        {
            boundsExtents = bounds.extents
        });
        AddComponent<ResourceCarried>();
        AddComponent<ResourceDropped>();
        AddComponent<LocalToWorld>();
    }
}