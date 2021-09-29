using Unity.Entities;
using UnityCamera = UnityEngine.Camera;

// Pay attention that this component is a class, in other words it's a managed component.
// Please check the package documentation to understand the implications of this.
[GenerateAuthoringComponent]
public class GameObjectRefs : IComponentData
{
    public UnityCamera Camera;
}