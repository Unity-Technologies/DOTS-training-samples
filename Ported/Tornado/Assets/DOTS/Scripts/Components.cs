using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Dots
{
    [Serializable]
    public struct BeamSpawnerData : IComponentData
    {
        public int buildingCount;
        public int debrisCount;
        public Entity beamPrefab;
    }

    [Serializable]
    public struct TransformMatrix : IComponentData
    {
        public float4x4 matrix;
    }

    [Serializable]
    public struct BeamData : IComponentData
    {
        public int p1;
        public int p2;
    }

    [Serializable]
    public struct Particle : IComponentData
    {
        public float radiusMult;
    }

    [Serializable]
    public struct TornadoConfigData : IComponentData
    {
        public Entity particlePrefab;
        public float3 position;
        public int particleCount;
        public int spinRate;
        public int upwardSpeed;
        public int initRange;
        public float force;
        public float maxForceDist;
        public float rotationModulation;
        public int height;
        public float upForce;
        public float inwardForce;
    }
}
