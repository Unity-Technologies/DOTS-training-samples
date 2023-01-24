using Authoring;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Aspects
{
    readonly partial struct CarAspect : IAspect
    {
        public readonly Entity Self;

        readonly RefRW<Car> Car;

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

    }
}
