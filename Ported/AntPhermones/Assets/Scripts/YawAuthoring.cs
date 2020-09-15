using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct Yaw : IComponentData
{
    public float CurrentYaw;
    public float CurrentYawVel;
}
