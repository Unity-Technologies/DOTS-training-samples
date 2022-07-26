using Aspects;
using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    [BurstCompile]
    partial struct CarTravelSystem : ISystem
    {
        
        [BurstCompile] public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile] public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile] 
        public void OnUpdate(ref SystemState state)
        {
            var dt = state.Time.DeltaTime;
            foreach (var car in SystemAPI.Query<CarAspect>())
            {
                car.T = math.clamp(car.T + dt, 0, 1);
                Track track;
                var trackEntity = state.GetComponentDataFromEntity<Track>(true);
                trackEntity.TryGetComponent(car.Track, out track);

                car.Position = track.Evaluate(car.T);
            }
        }


        
    }
}
