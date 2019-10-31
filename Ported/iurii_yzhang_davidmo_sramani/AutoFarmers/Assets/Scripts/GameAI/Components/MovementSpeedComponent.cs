using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace GameAI
{
    public struct MovementSpeedComponent : IComponentData 
    {
        public float speedInMeters;
    }
}