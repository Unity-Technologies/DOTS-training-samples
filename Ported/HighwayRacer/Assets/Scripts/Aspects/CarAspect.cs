using Authoring;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

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

        public float Length { get => Car.ValueRO.Length; }
        public float MaxSpeed { get => Car.ValueRO.MaxSpeed; }
        public float Acceleration { get => Car.ValueRO.Acceleration; }
        public int Index { get => Car.ValueRO.Index; }
    }
}
