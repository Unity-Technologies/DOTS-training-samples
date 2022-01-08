using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Combatbees.Testing.Maria
{
    [GenerateAuthoringComponent]
    public struct BloodParticle : IComponentData
    {
        public float3 direction;
    }
}