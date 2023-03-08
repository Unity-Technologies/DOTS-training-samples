using System.Drawing;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Aspects
{
    public readonly partial struct CarAspect : IAspect
    {
        public readonly Entity Self;

        public readonly RefRW<CarParameters> CarParameters;
        public readonly RefRW<CarData> CarData;
        readonly TransformAspect Transform;

        public float Distance
        {
            get => CarData.ValueRO.Distance;
            set => CarData.ValueRW.Distance = value;
        }

        public float Lane
        {
            get => CarData.ValueRO.Lane;
            set => CarData.ValueRW.Lane = value;
        }

        public float DesiredLane
        {
            get => CarData.ValueRO.DesiredLane;
            set => CarData.ValueRW.DesiredLane = value;
        }

        public float3 Position
        {
            get => Transform.LocalPosition;
            set => Transform.LocalPosition = value;
        }

        public readonly quaternion Rotation => Transform.LocalRotation;

        public readonly float Acceleration => CarParameters.ValueRO.Acceleration;

        public float Speed
        {
            get => CarData.ValueRO.Speed;
            set => CarData.ValueRW.Speed = value;
        }

        public float TEMP_NextLaneChangeCountdown
        {
            get => CarData.ValueRO.TEMP_NextLaneChangeCountdown;
            set => CarData.ValueRW.TEMP_NextLaneChangeCountdown = value;
        }

        public readonly float Length { get => CarParameters.ValueRO.Length; }

        public readonly float DesiredSpeed { get => CarParameters.ValueRO.DesiredSpeed; }
    }
    public readonly partial struct CarColorAspect : IAspect
    {
        public readonly Entity Self;

        public readonly RefRW<CarData> CarData;
        public readonly RefRW<CarColor> CarColor;

        public float PreviousDifferential
        {
            get => CarColor.ValueRO.PreviousDifferential;
            set => CarColor.ValueRW.PreviousDifferential = value;
        }

        public readonly float Speed
        {
            get => CarData.ValueRO.Speed;
            set => CarData.ValueRW.Speed = value;
        }
    }
}
