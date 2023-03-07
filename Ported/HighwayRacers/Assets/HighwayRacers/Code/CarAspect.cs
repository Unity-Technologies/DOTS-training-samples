using System.Drawing;
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

        public float PreviousDifferential
        {
            get => Car.ValueRO.PreviousDifferential;
            set => Car.ValueRW.PreviousDifferential = value;
        }

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

        public float DesiredLane
        {
            get => Car.ValueRO.DesiredLane;
            set => Car.ValueRW.DesiredLane = value;
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

        public float CruisingSpeed
        {
            get => Car.ValueRO.CruisingSpeed;
            set => Car.ValueRW.CruisingSpeed = value;
        }
        public float OvertakeSpeed
        {
            get => Car.ValueRO.OvertakeSpeed;
            set => Car.ValueRW.OvertakeSpeed = value;
        }


        public float OvertakeModeCountdown
        {
            get => Car.ValueRO.OvertakeModeCountdown;
            set => Car.ValueRW.OvertakeModeCountdown = value;
        }

        public float TEMP_NextLaneChangeCountdown
        {
            get => Car.ValueRO.TEMP_NextLaneChangeCountdown;
            set => Car.ValueRW.TEMP_NextLaneChangeCountdown = value;
        }

        public float Length { get => Car.ValueRO.Length; }
    }
}
