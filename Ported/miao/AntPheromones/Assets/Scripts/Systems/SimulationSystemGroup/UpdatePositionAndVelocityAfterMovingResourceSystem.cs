using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace AntPheromones_ECS
{
    [UpdateAfter(typeof(PickUpOrDropOffResourceSystem))]
    public class UpdatePositionAndVelocityAfterMovingResourceSystem : JobComponentSystem
    {
        private EntityQuery _mapQuery;

        protected override void OnCreate()
        {
            base.OnCreate();
            this._mapQuery = GetEntityQuery(ComponentType.ReadOnly<Map>());
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            JobHandle updateVelocityJob = new UpdateVelocityJob().Schedule(this, inputDeps);
            
            return new UpdatePositionJob
            {
                MapWidth = this._mapQuery.GetSingleton<Map>().Width,
            }.Schedule(this, updateVelocityJob);
        }
        
        [BurstCompile]
        private struct UpdateVelocityJob : IJobForEach<Speed, FacingAngle, Velocity>
        {
            public void Execute(
                [ReadOnly] ref Speed speed, 
                [ReadOnly] ref FacingAngle facingAngle, 
                [WriteOnly] ref Velocity velocity)
            {
                math.sincos(facingAngle.Value, out float sin, out float cos);
                velocity.Value = new float2(cos, sin) * speed.Value;
            }
        }
        
        [BurstCompile]
        private struct UpdatePositionJob : IJobForEach<Position, Velocity>
        {
            public float MapWidth;

            public void Execute(ref Position position, ref Velocity velocity)
            {
                if (position.Value.x + velocity.Value.x < 0f || position.Value.x + velocity.Value.x > this.MapWidth)
                {
                    velocity.Value.x = -velocity.Value.x;
                }
                else
                {
                    position.Value.x += velocity.Value.x;
                }
                if (position.Value.y + velocity.Value.y < 0f || position.Value.y + velocity.Value.y > this.MapWidth)
                {
                    velocity.Value.y = -velocity.Value.y;
                }
                else
                {
                    position.Value.y += velocity.Value.y;
                }
            }
        }
    }
}