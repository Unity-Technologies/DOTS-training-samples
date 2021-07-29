using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTSRATS
{
    public struct Arrow : IComponentData
    {
    }
    
    public static partial class DirectionExt
    {
        public static quaternion ToArrowRotation(this Direction dir)
        {
            switch (dir)
            {
                case Direction.North: return new quaternion(0.7071068f,0,0,0.7071068f);
                case Direction.East:  return new quaternion(0.5f,0.5f,-0.5f,0.5f);
                case Direction.South: return new quaternion(0,0.7071068f,-0.7071068f,0);
                case Direction.West:  return new quaternion(-0.5f,0.5f,-0.5f,-0.5f);
                default:              return quaternion.identity;
                
            }
        }
    }
}