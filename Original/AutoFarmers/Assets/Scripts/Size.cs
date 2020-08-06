using Unity.Entities;
using Unity.Transforms;

[WriteGroup(typeof(LocalToWorld))]
public struct Size : IComponentData
{
    public float value;
}
