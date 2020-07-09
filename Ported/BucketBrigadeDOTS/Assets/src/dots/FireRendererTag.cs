using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

// Fire simulation grid settings
[GenerateAuthoringComponent]
[WriteGroup(typeof(LocalToWorld))]
public struct FireRendererTag : IComponentData
{
}
