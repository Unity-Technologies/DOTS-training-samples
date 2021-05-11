// Pay attention that this component is a class, in other words it's a managed component.
// Please check the package documentation to understand the implications of this.
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public class GameObjectRefs : IComponentData
{
    public Camera Camera;
}