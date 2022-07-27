using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace Aspects
{
    public readonly partial struct CarAspect : IAspect<CarAspect>
    {
        readonly RefRW<Car> m_Car;
        readonly TransformAspect m_TransformAspect;
        readonly RefRW<URPMaterialPropertyBaseColor> m_BaseColor;

        public float3 Position
        {
            get => m_TransformAspect.Position;
            set => m_TransformAspect.Position = value;
        }

        public float4 Color
        {
            get => m_BaseColor.ValueRO.Value;
            set => m_BaseColor.ValueRW.Value = value;
        }

        public float3 Direction
        {
            get => m_TransformAspect.Forward;
        }

        public float Speed
        {
            get => m_Car.ValueRO.Speed;
            set => m_Car.ValueRW.Speed = value;
        }

        public float T
        {
            get => m_Car.ValueRO.T;
            set => m_Car.ValueRW.T = value;
        }
        
        public Entity RoadSegment
        {
            get => m_Car.ValueRO.RoadSegment;
            set => m_Car.ValueRW.RoadSegment = value;
        }
    }
}
