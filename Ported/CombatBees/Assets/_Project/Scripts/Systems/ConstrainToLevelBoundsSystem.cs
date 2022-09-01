using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateBefore(typeof(TransformSystemGroup))]
public partial class ConstrainToLevelBoundsSystem : SystemBase
{
    protected override void OnUpdate()
    {
        if (!HasSingleton<GameRuntimeData>())
            return;
        
        GameRuntimeData runtimeData = GetSingleton<GameRuntimeData>();
        Box levelBounds = runtimeData.GridCharacteristics.LevelBounds;
        
        Entities
            .WithAll<ConstrainToLevelBounds>()
            .ForEach((ref Translation translation) =>
            {
                translation.Value = levelBounds.GetClosestPoint(translation.Value);
            }).ScheduleParallel();
    }
}
