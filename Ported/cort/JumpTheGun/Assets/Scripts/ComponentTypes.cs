using System;
using Unity.Entities;
using Unity.Mathematics;

namespace JumpTheGun {
    // Use a 2D position for block entities, mainly to avoid the default
    // EndFrameTransformSystem (in favor of BlockTransformSystem).
    struct BlockPositionXZ : IComponentData
    {
        public float2 Value;
    }
    struct BlockHeight : IComponentData
    {
        public float Value;
    }
    // Index of a block in the terrain cache array.
    struct BlockIndex : IComponentData
    {
        public int Value;
    }

    struct ArcState : IComponentData
    {
        public float3 Parabola; // a, b, c parameters
        public float StartTime;
        public float Duration;
        public float3 StartPos;
        public float3 EndPos;
    }

    struct TankFireCountdown : IComponentData
    {
        public float SecondsUntilFire;
    };

    // Marks blocks whose LocalToWorld matrix need to be updated after
    // their height changes.`
    struct UpdateBlockTransformTag : IComponentData {}
    // Tags to differentiate tank base & tank cannon entities.
    // TODO(@cort): use IJobChunk to filter based on MeshInstanceRenderer value instead?
    struct TankBaseTag : IComponentData {}
    struct TankCannonTag : IComponentData {}
}
