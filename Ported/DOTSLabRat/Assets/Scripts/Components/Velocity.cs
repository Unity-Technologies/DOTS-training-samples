using System;
using Unity.Entities;
using Unity.Mathematics;

namespace DOTSRATS
{
    [GenerateAuthoringComponent]
    public struct Velocity : IComponentData
    {
        public Direction Direction;
        public float Speed;
    }

    public static class DirectionExt
    {
        public static float3 ToFloat3(this Direction dir)
        {
            switch (dir)
            {
                case Direction.North: return new float3( 0f,  0f, +1f);
                case Direction.South: return new float3( 0f,  0f, -1f);
                case Direction.East:  return new float3(+1f,  0f,  0f);
                case Direction.West:  return new float3(-1f,  0f,  0f);
                case Direction.Up:    return new float3( 0f, +1f,  0f);
                case Direction.Down:  return new float3( 0f, -1f,  0f);
                case Direction.None:
                default:              return float3.zero;
                
            }
        }
    }
}
