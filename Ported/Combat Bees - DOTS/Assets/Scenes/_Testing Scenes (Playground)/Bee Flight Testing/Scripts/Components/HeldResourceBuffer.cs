using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace CombatBees.Testing.BeeFlight
{
    [GenerateAuthoringComponent]
    public struct HeldResourceBuffer : IBufferElementData
    {
        public Entity Bee;
        public Entity Resource;
    }
}