using Unity.Entities;
using Components;

namespace Aspects
{
    readonly partial struct RoadSegmentAspect : IAspect<RoadSegmentAspect>
    {
        private readonly RefRW<RoadSegment> m_roadSegment;

        public Entity StartIntersection
        {
            get => m_roadSegment.ValueRO.StartIntersection;
        }
        
        public Entity EndIntersection
        {
            get => m_roadSegment.ValueRO.EndIntersection;
        }
        
    }
}
