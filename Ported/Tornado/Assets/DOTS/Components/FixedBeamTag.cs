using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Dots
{
    // This tag indicate that the beam can not move (fixed on the ground)
    // With this tag we can skip updating that beam's transform
    [Serializable]
    public struct FixedBeamTag : IComponentData
    {
    }
}
