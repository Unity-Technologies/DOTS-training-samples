using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

/// <summary>
/// Constant parameters for the simulation
/// </summary>
[GenerateAuthoringComponent, Serializable]
public struct SimulationParameters : IComponentData
{
    // Hands
    public float ArmSeparation;
    public float ArmJointLength;
    public float ArmJointSpacing;

    // Can carousel
    public float CanScrollSpeed;
    public float CanScrollDepth;
    public float CanScrollMinHeight;
    public float CanScrollMaxHeight;
    public float CanMinSize;
    public float CanMaxSize;

    // Rock carousel
    public float RockScrollSpeed;
    public float RockScrollDepth;
    public float RockMinSize;
    public float RockMaxSize;

    /// Time it takes from spawning to being fully scaled up, in seconds
    public float ScaleUpTime;

    // Camera
    public float CameraSwayTiltDuration;
    public float CameraTiltAmount;
    public float CameraTiltOffset;
    public float CameraSpinDuration;
    public float CameraSpinAmount;
    public float CameraZoomDuration;
    public float CameraMinCamDist;
    public float CameraMaxCamDist;
    public float CameraSwayXDuration;
    public float3 CameraPosOffset;
}
