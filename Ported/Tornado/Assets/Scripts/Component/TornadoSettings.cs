using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
public class TornadoSettings : IComponentData
{
    public float Damping;
    public float Friction;
    public float BreakResistance;
    public float ExpForce;
    public float TornadoForce;
    public float TornadoMaxForceDistance;
    public float TornadoHeight;
    public float TornadoUpForce;
    public float TornadoInwardForce;
    public float SpinRate;
    public float SpeedUpward;
}
