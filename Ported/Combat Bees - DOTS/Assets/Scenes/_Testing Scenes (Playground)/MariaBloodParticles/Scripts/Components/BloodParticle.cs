using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Random = System.Random;

namespace Combatbees.Testing.Maria
{
    [GenerateAuthoringComponent]
    public struct BloodParticle : IComponentData
    {
        public float3 direction;
        public float timeToLive;
        public float steps;
    }
}