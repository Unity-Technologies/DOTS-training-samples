using Aspects;
using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Systems
{
    [WithAll(typeof(Car))]
    [BurstCompile]
    partial struct CarPositionJob : IJobEntity
    {
        [ReadOnly] public ComponentDataFromEntity<RoadSegment> RoadSegmentFromEntity;
        public float DT;

        void Execute(ref CarAspect carAspect)
        {
            float speedDelta = 0f;
            // if (carAspect.IsBraking())
            // {
            //     speedDelta = -0.2f;
            // }
            // else
            // {
                speedDelta = 0.2f;
            // }

            carAspect.Speed = math.clamp(carAspect.Speed + (speedDelta * DT), 0f, 10f);

            carAspect.T = math.clamp(carAspect.T + (carAspect.Speed * DT), 0, 1);
            RoadSegmentFromEntity.TryGetComponent(carAspect.RoadSegment, out RoadSegment track);

            var anchor1 = track.StartPos + track.StartTang;
            var anchor2 = track.EndPos - track.EndTang;
            carAspect.Position = track.StartPos * (1f - carAspect.T) * (1f - carAspect.T) * (1f - carAspect.T) + 3f * anchor1 * (1f - carAspect.T) * (1f - carAspect.T) * carAspect.T + 3f * anchor2 * (1f - carAspect.T) * carAspect.T * carAspect.T + track.EndPos * carAspect.T * carAspect.T * carAspect.T;
        }
    }

    [BurstCompile]
    partial struct CarTravelSystem : ISystem
    {
        ComponentDataFromEntity<RoadSegment> m_RoadSegmentFromEntity;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            m_RoadSegmentFromEntity = state.GetComponentDataFromEntity<RoadSegment>(true);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            m_RoadSegmentFromEntity.Update(ref state);

            var dt = state.Time.DeltaTime;

            var brakeJob = new CarPositionJob
            {
                DT = dt,
                RoadSegmentFromEntity = m_RoadSegmentFromEntity
            };
            brakeJob.ScheduleParallel();
        }
    }
}
