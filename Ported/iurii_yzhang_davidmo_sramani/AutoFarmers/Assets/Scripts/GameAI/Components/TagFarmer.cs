using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace GameAI
{
    public struct HasTarget : IComponentData
    {
        public int2 TargetPosition;

        public HasTarget(int2 pos)
        {
            TargetPosition = pos;
        }
    }

    public struct TagFarmer : IComponentData
    {
    };
}