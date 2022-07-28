using System.Linq;
using Aspects;
using Components;
using Unity.Burst;
using Unity.Entities;

namespace Systems
{
    [BurstCompile]
    
    
    
    partial struct IntersectionOccupiedJob:IJobEntity
    {

        void Execute(ref CarAspect car)
        {
            
        }
    }
    
    [BurstCompile]
    partial struct IntersectionOccupiedSystem:ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

        }
    }
}
