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
                GetEntityQuery(ComponentType.ReadOnly<AntIndividualRendering>());
            this._mapQuery = GetEntityQuery(ComponentType.ReadOnly<Map>());
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var ant = this._antRenderingQuery.GetSingleton<AntIndividualRendering>();
            var map = this._mapQuery.GetSingleton<Map>();
            
            return new Job
            {
                MapWidth = map.Width,
                Scale = ant.Scale
            }.Schedule(this, inputDeps);
        }

        [BurstCompile]
        struct Job : IJobForEach<FacingAngle, Position, LocalToWorld>
        {
            public float3 Scale;
            public int MapWidth;

            public void Execute(
                [ReadOnly] ref FacingAngle angle, 
                [ReadOnly] ref Position position,
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
