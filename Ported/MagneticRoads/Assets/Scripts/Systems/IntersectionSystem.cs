using System.Linq;
using Aspects;
using Components;
using Unity.Burst;
using Unity.Entities;

namespace Systems
{
    
    partial struct IntersectionJob:IJobEntity
    {
        public ComponentDataFromEntity<Occupied> IntersectionOccupiedFromEntity;
        public ComponentDataFromEntity<Intersection> IntersectionData;

        void Execute(ref CarAspect car)
        {
            if (car.T >= 1)
            {
               /* var entity = car.EndIntersectionFromRoadSegment;
                if (!IntersectionOccupiedFromEntity.IsComponentEnabled(entity))
                {
                    var intersectionComponent = IntersectionData.GetRefRW(entity, false);

                    intersectionComponent.ValueRW.CurrentCar = car.Entity;
                    IntersectionOccupiedFromEntity.SetComponentEnabled(entity, true);
                    
                    //give car new roadsegment
                    //set car T to 0
                    
                    // car.RoadSegment = intersectionComponent.ValueRO.
                }*/
            }
        }
    }
    
    //[BurstCompile]
    partial struct IntersectionSystem:ISystem
    {
        
        ComponentDataFromEntity<Occupied> m_IntersectionOccupiedFromEntity;
        ComponentDataFromEntity<Intersection> m_IntersectionData;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            m_IntersectionOccupiedFromEntity = state.GetComponentDataFromEntity<Occupied>(false);
            m_IntersectionData = state.GetComponentDataFromEntity<Intersection>(false);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            
            m_IntersectionOccupiedFromEntity.Update(ref state);
            m_IntersectionData.Update(ref state);
            var intersectionJob = new IntersectionJob()
            {
                IntersectionOccupiedFromEntity = m_IntersectionOccupiedFromEntity,
                IntersectionData = m_IntersectionData
            };
            intersectionJob.ScheduleParallel();
        }
    }
}
