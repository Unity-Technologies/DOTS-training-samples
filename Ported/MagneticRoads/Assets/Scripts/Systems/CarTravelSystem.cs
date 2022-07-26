using System.ComponentModel;
using Aspects;
using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace Systems
{
    [WithAll(typeof(Car))]
    [BurstCompile]
    partial struct CarBrakingJob : IJobEntity
    {
        // [ReadOnly(true)] public ComponentDataFromEntity<Car> CarFromEntity;
        public EntityCommandBuffer ECB;
        public float DT;

        void Execute(Entity e, in CarAspect carAspect)
        {
            float speedDelta = 0f;
            if (carAspect.IsBraking())
            {
                speedDelta = -0.2f;
            }
            else
            {
                speedDelta = 0.2f;
            }

            var speed = math.clamp(carAspect.Speed + (speedDelta * DT), 0f, 10f);

            ECB.SetComponent(e, new Car
            {
                T = carAspect.T,
                SafeDistance = 0, // Just FAFO
                Track = carAspect.Track,
                Speed = speed
            });
        }
    }

    [BurstCompile]
    partial struct CarTravelSystem : ISystem
    {
        ComponentDataFromEntity<Track> m_TrackComponentFromEntity;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            m_TrackComponentFromEntity = state.GetComponentDataFromEntity<Track>(true);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            m_TrackComponentFromEntity.Update(ref state);

            var dt = state.Time.DeltaTime;
            foreach (var car in SystemAPI.Query<CarAspect>())
            {
                car.T = math.clamp(car.T * car.Speed * dt, 0, 1);
                m_TrackComponentFromEntity.TryGetComponent(car.Track, out Track track);

                var anchor1 = track.StartPos + track.StartTang;
                var anchor2 = track.EndPos - track.EndTang;
                car.Position = track.StartPos * (1f - car.T) * (1f - car.T) * (1f - car.T) + 3f * anchor1 * (1f - car.T) * (1f - car.T) * car.T + 3f * anchor2 * (1f - car.T) * car.T * car.T + track.EndPos * car.T * car.T * car.T;
            }

            var brakeJob = new CarBrakingJob
            {
                ECB = ecb,
                DT = dt,
            };
            brakeJob.ScheduleParallel();
        }
    }
}
