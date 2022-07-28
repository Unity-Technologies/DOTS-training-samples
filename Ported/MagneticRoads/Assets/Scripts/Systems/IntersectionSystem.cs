using Aspects;
using Components;
using Unity.Entities;

namespace Systems
{
    partial struct IntersectionSystem:ISystem
    {
        
        public void OnCreate(ref SystemState state)
        {
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            RoadSegment RoadSegmentData;

            foreach (var car in SystemAPI.Query<CarAspect>())
            {
                if (car.T >= 1)
                {
                    RoadSegmentData = SystemAPI.GetComponent<RoadSegment>(car.RoadSegment);
                    
                }
            }
        }
    }
}
