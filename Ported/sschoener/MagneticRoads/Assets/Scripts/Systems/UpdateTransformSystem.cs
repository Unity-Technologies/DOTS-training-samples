using System;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems {
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(UpdateCoordinatesSystem))]
    public class UpdateTransformSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return Entities.ForEach((ref LocalToWorld ltw, in CoordinateSystemComponent coords) =>
            {
                var q = quaternion.LookRotation(coords.Forward, coords.Up);
                float3 scale = new float3(.1f, .08f, .12f);
                ltw.Value = float4x4.TRS(coords.Position, q, scale);
            }).Schedule(inputDeps);
        }
    }
}
