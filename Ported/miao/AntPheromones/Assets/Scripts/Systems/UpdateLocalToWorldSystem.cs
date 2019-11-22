using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace AntPheromones_ECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(DropPheromoneSystem))]
    public class UpdateLocalToWorldSystem : JobComponentSystem
    {
        private EntityQuery _antRenderingQuery;
        private EntityQuery _mapQuery;

        protected override void OnCreate()
        {
            base.OnCreate();
            
            this._antRenderingQuery = 
                GetEntityQuery(ComponentType.ReadOnly<AntRenderingComponent>());
            this._mapQuery = GetEntityQuery(ComponentType.ReadOnly<MapComponent>());
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var ant = this._antRenderingQuery.GetSingleton<AntRenderingComponent>();
            var map = this._mapQuery.GetSingleton<MapComponent>();
            
            return new Job
            {
                MapWidth = map.Width,
                Scale = ant.Scale
            }.Schedule(this, inputDeps);
        }

        [BurstCompile]
        struct Job : IJobForEach<FacingAngleComponent, PositionComponent, LocalToWorld>
        {
            public float3 Scale;
            public int MapWidth;

            public void Execute(
                [ReadOnly] ref FacingAngleComponent angle, 
                [ReadOnly] ref PositionComponent position,
                [WriteOnly] ref LocalToWorld localToWorld)
            {
                localToWorld.Value = float4x4.TRS(
                    new float3(position.Value, 0) / this.MapWidth,
                    quaternion.Euler(new float3(0, 0, angle.Value)),
                    Scale);
            }
        }
    }

}
