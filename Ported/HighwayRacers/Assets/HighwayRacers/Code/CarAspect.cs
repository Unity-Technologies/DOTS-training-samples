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

        public float Distance
        {
            get => Car.ValueRO.Distance;
            set => Car.ValueRW.Distance = value;
        }

        public float Lane
        {
            get => Car.ValueRO.Lane;
            set => Car.ValueRW.Lane = value;
        }

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

        public float Length { get => Car.ValueRO.Length; }

        public float DesiredSpeed { get => Car.ValueRO.DesiredSpeed; }
    }
}
