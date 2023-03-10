using System.Drawing;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Aspects
{
    public readonly partial struct CarAspect : IAspect
    {
        public readonly Entity Self;

        public readonly RefRW<CarPosition> CarPosition;
        public readonly RefRW<CarParameters> CarParameters;
        public readonly RefRW<CarData> CarData;
        readonly TransformAspect Transform;

        public float Distance
        {
            get => CarPosition.ValueRO.Distance;
            set => CarPosition.ValueRW.Distance = value;
        }

        public float CurrentLane
        {
            get => CarPosition.ValueRO.CurrentLane;
            set => CarPosition.ValueRW.CurrentLane = value;
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

        public readonly float Length => CarParameters.ValueRO.Length;

        public float Speed
        {
            get => CarPosition.ValueRO.Speed;
            set => CarPosition.ValueRW.Speed = value;
        }

        public readonly float CruisingSpeed
        {
            get => CarParameters.ValueRO.CruisingSpeed;
        }
        public readonly float OvertakeSpeed
        {
            get => CarParameters.ValueRO.OvertakeSpeed;
        }
        public float OvertakeModeReturnToLane
        {
            get => CarData.ValueRO.OvertakeModeReturnToLane;
            set => CarData.ValueRW.OvertakeModeReturnToLane = value;
        }

        public float OvertakeModeCountdown
        {
            get => CarData.ValueRO.OvertakeModeCountdown;
            set => CarData.ValueRW.OvertakeModeCountdown = value;
        }

        public readonly float DesiredSpeed { get => CarParameters.ValueRO.DesiredSpeed; }

    }
    public readonly partial struct CarColorAspect : IAspect
    {
        public readonly Entity Self;

        public readonly RefRW<CarPosition> CarPosition;
        public readonly RefRW<CarParameters> CarParameters;
        public readonly RefRW<LaneChangeState> LaneChangeState;
        public readonly RefRW<CarColor> CarColor;

        public float PreviousDifferential
        {
            get => CarColor.ValueRO.PreviousDifferential;
            set => CarColor.ValueRW.PreviousDifferential = value;
        }

        public readonly float Speed
        {
            get => CarPosition.ValueRO.Speed;
        }

        public readonly float CruisingSpeed
        {
            get => CarParameters.ValueRO.CruisingSpeed;
        }

        public readonly float DistFromCarInFront
        {
            get => LaneChangeState.ValueRO.distFromCarInFront;
        }
    }
}
