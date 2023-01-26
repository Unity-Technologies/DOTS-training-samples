using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Aspects
{
    readonly partial struct CarAspect : IAspect
    {
        public readonly Entity Self;

        public readonly RefRW<Car> Car;

        readonly TransformAspect Transform;

        public float Distance
        {
            get => Car.ValueRO.Distance;
            set => Car.ValueRW.Distance = value;
        }

        public int LaneNumber
        {
            get => Car.ValueRO.LaneNumber;
            set => Car.ValueRW.LaneNumber = value;
        }

        public int NewLaneNumber
        {
            get => Car.ValueRO.NewLaneNumber;
            set => Car.ValueRW.NewLaneNumber = value;
        }

        public float LaneChangeProgress
        {
            get => Car.ValueRO.LaneChangeProgress;
            set => Car.ValueRW.LaneChangeProgress = value;
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

        public float Speed
        {
            get => Car.ValueRO.Speed;
            set => Car.ValueRW.Speed = value;
        }

        public float Acceleration
        {
            get => Car.ValueRO.Acceleration;
            set => Car.ValueRW.Acceleration = value;
        }
        public float Length { get => Car.ValueRO.Length; }
        public float MaxSpeed { get => Car.ValueRO.MaxSpeed; }
        public int Index { get => Car.ValueRO.Index; }
    }
}
