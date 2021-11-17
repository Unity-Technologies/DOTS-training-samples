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
        public float3 p1;
        public float3 p2;
    }
}
