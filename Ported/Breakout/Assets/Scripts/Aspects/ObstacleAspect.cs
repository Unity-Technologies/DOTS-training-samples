using Authoring;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Aspects
{
    readonly partial struct ObstacleAspect : IAspect
    {
        public readonly Entity Self;

		readonly TransformAspect Transform;

        readonly RefRW<Cylinder> Obstacle;

        public float3 Position
        {
            get => Transform.LocalPosition;
            set => Transform.LocalPosition = value;
        }

        public float Radius
        {
            get => Transform.LocalScale * 0.5f;
            set => Transform.LocalScale = value * 2.0f;
        }

    }
}
