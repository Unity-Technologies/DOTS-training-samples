using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace AntPheromones_ECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(DropPheromoneSystem))]
    public class UpdateLocalToWorldSystem : JobComponentSystem
    {
        private AntRenderingComponent _antRenderingComponent;

        protected override void OnCreate()
        {
            base.OnCreate();
            this._antRenderingComponent = 
                GetEntityQuery(ComponentType.ReadOnly<AntRenderingComponent>()).GetSingleton<AntRenderingComponent>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new Job
            {
                Scale = this._antRenderingComponent.Scale
            }.Schedule(this, inputDeps);
        }

        [BurstCompile]
        struct Job : IJobForEach<FacingAngleComponent, PositionComponent, LocalToWorldComponent>
        {
            public float3 Scale;

            public void Execute(
                [ReadOnly] ref FacingAngleComponent angle, 
                [ReadOnly] ref PositionComponent position,
                [WriteOnly] ref LocalToWorldComponent localToWorld)
            {
                localToWorld.Value = float4x4.TRS(
                    new float3(position.Value, 0),
                    quaternion.Euler(new float3(0, 0, angle.Value)),
                    Scale);
            }
        }
    }

}
