using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial class RailMarkerGenerationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var config = SystemAPI.GetSingleton<Config>();

        Entities
            .WithDeferredPlaybackSystem<BeginSimulationEntityCommandBufferSystem>()
            .ForEach((BezierPath bezierPath, EntityCommandBuffer ecb) =>
            {
                var railSpacingInBezierSpace = Bezier.GetRelativeSizeOnBezier(ref bezierPath.Data.Value, config.RailSpacing);
                var railCount = bezierPath.Data.Value.distance / railSpacingInBezierSpace;
                ref var points = ref bezierPath.Data.Value.Points;

                for (var r = 0; r < railCount; r++)
                {
                    var splinePos = railSpacingInBezierSpace * r;

                    var railEntity = ecb.Instantiate(config.RailPrefab);

                    var railPosition = Bezier.GetPosition(ref points, splinePos);
                    ecb.SetComponent(railEntity, new Translation { Value = railPosition });
                    
                    var lookAtPoint = Bezier.GetLookAtTarget(ref points, splinePos);
                    var targetDir = lookAtPoint - railPosition;
                    var railRotation = quaternion.LookRotationSafe(targetDir, math.up());
                    ecb.SetComponent(railEntity, new Rotation { Value = railRotation });
                }
            }).ScheduleParallel();

        Enabled = false;
    }
}