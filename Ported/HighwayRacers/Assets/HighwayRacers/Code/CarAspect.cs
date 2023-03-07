using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Aspects
{
    public readonly partial struct CarAspect : IAspect
    {
        public readonly Entity Self;

        public readonly RefRW<Car> Car;

        readonly TransformAspect Transform;

        public float3 Position
        {
            get => Transform.LocalPosition;
            set => Transform.LocalPosition = value;
        }

        public quaternion Rotation
        {
            get => Transform.LocalRotation;
            set => Transform.LocalRotation = value;
        }

        public float Acceleration
        {
            get => Car.ValueRO.Acceleration;
            set => Car.ValueRW.Acceleration = value;
        }

        public float Speed
        {
            get => Car.ValueRO.Speed;
            set => Car.ValueRW.Speed = value;
        }

        public float DesiredSpeed { get => Car.ValueRO.DesiredSpeed; }
    }
}
