using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace CombatBees.Testing.BeeFlight
{


   [GenerateAuthoringComponent]
    public struct Test : IComponentData
    {
        public Entity Value;


    }
}