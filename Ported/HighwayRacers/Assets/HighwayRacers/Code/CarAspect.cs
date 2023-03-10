using System.Drawing;
using HighwayRacers;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Aspects
{
    public readonly partial struct CarAspect : IAspect
    {
        public readonly Entity Self;

        public readonly RefRO<CarParameters> CarParameters;
        public readonly RefRW<CarData> CarData;
        readonly TransformAspect Transform;

        public float Distance
        {
            get => CarData.ValueRO.Distance;
            set => CarData.ValueRW.Distance = value;
        }

        public float CurrentLane
        {
            get => CarData.ValueRO.CurrentLane;
            set => CarData.ValueRW.CurrentLane = value;
        }

        public float DesiredLane
        {
            get => CarData.ValueRO.DesiredLane;
            set => CarData.ValueRW.DesiredLane = value;
        }

        public float3 Position
        {
            get => Transform.WorldPosition;
            set => Transform.WorldPosition = value;
        }
        
        public quaternion Rotation
        {
            get => Transform.LocalRotation;
            set => Transform.LocalRotation = value;
        }

        public readonly float Acceleration => CarParameters.ValueRO.Acceleration;

        public readonly float Length => CarParameters.ValueRO.Length;

        public float Speed
        {
            get => CarData.ValueRO.Speed;
            set => CarData.ValueRW.Speed = value;
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

        public float TEMP_NextLaneChangeCountdown
        {
            get => CarData.ValueRO.TEMP_NextLaneChangeCountdown;
            set => CarData.ValueRW.TEMP_NextLaneChangeCountdown = value;
        }

        public readonly float DesiredSpeed
        {
            get => CarData.ValueRO.DesiredSpeed;
            set => CarData.ValueRW.DesiredSpeed = value;
        }

    }
    public readonly partial struct CarColorAspect : IAspect
    {
        public readonly Entity Self;

        public readonly RefRO<CarData> CarData;
        public readonly RefRW<CarColor> CarColor;

        public float PreviousDifferential
        {
            get => CarColor.ValueRO.PreviousDifferential;
            set => CarColor.ValueRW.PreviousDifferential = value;
        }

        public readonly float Speed
        {
            get => CarData.ValueRO.Speed;
        }

        public readonly float DesiredSpeed
        {
            get => CarData.ValueRO.DesiredSpeed;
        }
    }
}
