using System.Linq;
using Aspects;
using Components;
using Unity.Burst;
using Unity.Entities;

namespace Systems
{
    [BurstCompile]
    partial struct IntersectionOccupiedJob : IJobEntity
    {
        void Execute(ref CarAspect car) { }
    }

    [UpdateAfter(typeof(EvaluateCarsNextIntersectionSystem))]
    [BurstCompile]
    partial struct IntersectionOccupiedSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            //need to get rid of this to work lol
            state.Enabled = false;
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var carAspect in SystemAPI.Query<CarAspect>().WithAll<WaitingAtIntersection>().WithNone<TraversingIntersection>())
            {
                var intersection = SystemAPI.GetComponent<Intersection>(carAspect.NextIntersection);
                if (intersection.IsOccupied)
                {
                    ecb.SetComponentEnabled<WaitingAtIntersection>(carAspect.Entity, false);
                }
                else
                {
                    intersection.IsOccupied = true;
                }
            }
        }
    }
}

