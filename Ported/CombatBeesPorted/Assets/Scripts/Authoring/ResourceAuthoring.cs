using Unity.Entities;
using UnityEngine;

class ResourceAuthoring : UnityEngine.MonoBehaviour
{
}

class ResourceBaker : Baker<ResourceAuthoring>
{
    public override void Bake(ResourceAuthoring authoring)
    {
        AddComponent(new Resource());
    }
}