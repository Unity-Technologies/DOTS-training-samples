using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
[GenerateAuthoringComponent]
public struct EntityCamera : IComponentData
{
    public float RotationSpeed;
    public float ZoomSpeed;
    public float MinZoomDistance;
    public float MaxZoomDistance;
    public float CenterHeight;

    public bool IsInitialized;
    public float CurrentZoomDistance;
    public float PitchAngle;
    public float YawAngle;
}
