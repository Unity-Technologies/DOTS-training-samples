using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

// Pay attention that this component is a class, in other words it's a managed component.
// Please check the package documentation to understand the implications of this.
[GenerateAuthoringComponent]
public class GameObjectRefs : IComponentData
{
    public Camera Camera;
    public float Sensitivity;
    public float ZoomSensitivity;
    public float Stiffness;

    public float ViewDist;
    public float SmoothViewDist;
    public Vector2 ViewAngles;
    public Vector2 SmoothViewAngles;
    
}
