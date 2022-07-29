using Components;
using Unity.Entities;
using Unity.Mathematics;

namespace Aspects
{
    public readonly partial struct RoadSegmentAspect : IAspect<RoadSegmentAspect>
    {
        readonly RefRW<RoadSegment> m_RoadSegment;
        public readonly Entity Entity;

        public Entity StartIntersection
        {
            get => m_RoadSegment.ValueRO.StartIntersection;
            set => m_RoadSegment.ValueRW.StartIntersection = value;
        }
        
        public Entity EndIntersection
        {
            get => m_RoadSegment.ValueRO.EndIntersection;
            set => m_RoadSegment.ValueRW.EndIntersection = value;
        }

        public RoadSegment RoadSegment
        {
            get => m_RoadSegment.ValueRO;
        }

        public float3 StartPosition
        {
            get => m_RoadSegment.ValueRO.Start.Position;
        }

        public float3 EndPosition
        {
            get => m_RoadSegment.ValueRO.End.Position;
        }
    }
}
