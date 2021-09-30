using dots_src.Components;
using Unity.Collections;
using Unity.Entities;

namespace dots_src.Systems
{
    public partial class BoardingSystem : SystemBase {
        EndSimulationEntityCommandBufferSystem m_SimulationECBSystem;
        protected override void OnCreate()
        {
            m_SimulationECBSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;
            var ecb = m_SimulationECBSystem.CreateCommandBuffer().AsParallelWriter();
            
            Entities.ForEach((int entityInQueryIndex, ref Occupancy occupancy) =>
            {
                if (occupancy.Train == Entity.Null) return;
                
                if (occupancy.TimeLeft > 0) 
                    occupancy.TimeLeft -= deltaTime;
                else
                {
                    ecb.SetComponent(entityInQueryIndex, occupancy.Train, 
                        new TrainState{State = TrainMovementStates.Starting});
                    occupancy.Train = Entity.Null;
                }
            }).ScheduleParallel();
            
            m_SimulationECBSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
