using System;
using Unity.Entities;
using UnityEngine;

public struct ParticleSharedData : ISharedComponentData, IEquatable<ParticleSharedData>
{
    public Mesh particleMesh;
    public Material particleMaterial;
    public float spinRate;
    public float upwardSpeed;
    public MaterialPropertyBlock matProps;
    public float tornadoSway;

    public bool Equals(ParticleSharedData other) {
        return spinRate == other.spinRate && upwardSpeed == other.upwardSpeed;
    }

    public override int GetHashCode() {
        return particleMesh.GetHashCode();
    }
}
