using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Aspects
{
    public readonly partial struct CarAspect : IAspect<CarAspect>
    {
        readonly RefRW<Car> m_Car;
        readonly TransformAspect m_TransformAspect;

        public float3 Position
        {
            get => m_TransformAspect.Position;
            set => m_TransformAspect.Position = value;
        }

        public float3 Direction
        {
            get => m_TransformAspect.Forward;
        }

        public float Speed
        {
            get => m_Car.ValueRO.Speed;
        }
    }
}
