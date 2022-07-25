using Components;
using Unity.Entities;
using Unity.Mathematics;

namespace Aspects
{
    readonly partial struct CarAspect : IAspect<CarAspect>
    {
        readonly RefRW<Car> m_Car;

        public float3 Position
        {
            get => m_Car.ValueRO.Position;
            set => m_Car.ValueRW.Position = value;
        }

        public float Speed
        {
            get => m_Car.ValueRO.Speed;
        }

        public float3 Direction
        {
            get => m_Car.ValueRO.Direction;
        }
    }
}
