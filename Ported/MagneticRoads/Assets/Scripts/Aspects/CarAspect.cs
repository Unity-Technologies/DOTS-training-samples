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
        readonly RefRO<Braking> m_Braking;

        public float3 Position
        {
            get => m_TransformAspect.Position;
            set => m_TransformAspect.Position = value;
        }

        public bool IsBraking()
        {
            return m_Braking.IsValid;
        }

        public float3 Direction
        {
            get => m_TransformAspect.Forward;
        }

        public float Speed
        {
            get => m_Car.ValueRO.Speed;
        }

        public float T
        {
            get => m_Car.ValueRO.T;
            set => m_Car.ValueRW.T = value;
        }
        
        public Entity Track
        {
            get => m_Car.ValueRO.Track;
            set => m_Car.ValueRW.Track = value;
        }
    }
}
