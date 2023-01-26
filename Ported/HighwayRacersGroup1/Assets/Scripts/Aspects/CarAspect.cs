using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace Aspects
{
    readonly partial struct CarAspect : IAspect
    {
        public readonly Entity CarEntity;
        readonly RefRO<CarData> m_CarData;

        public float Speed => m_CarData.ValueRO.Speed;
        public float DefaultSpeed => m_CarData.ValueRO.DefaultSpeed;
    }
}