using System;
using UnityEngine;
using Unity.Entities;


[Serializable]
public struct Resource: IComponentData
{

}

public class ResourceAuthoring : MonoBehaviour
{
    public class Baked : Baker<ResourceAuthoring>
    {
        public override void Bake(ResourceAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Renderable);
            AddComponent<Resource>(entity);
        }
    }
}
