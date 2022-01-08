using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Combatbees.Testing.Maria
{
    [GenerateAuthoringComponent]
    public struct Bee : IComponentData
    {
        public bool dead;
    }
}