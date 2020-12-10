using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct TornadoSettings : IComponentData
{
    [Range(0f, 1f)]
    public float Damping;
    [Range(0f, 1f)]
    public float Friction;
    public float BreakResistance;
    [Range(0f, 1f)]
    public float TornadoForce;
    public float TornadoMaxForceDistance;
    public float TornadoHeight;
    public float TornadoUpForce;
    public float TornadoInwardForce;
    public float SpinRate;
    public float SpeedUpward;
}
