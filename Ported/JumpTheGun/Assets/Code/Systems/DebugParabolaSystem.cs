using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[UpdateBefore(typeof(BulletMovementSystem))]
public class DebugParabolaSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entity playerEntity;
        if (!TryGetSingletonEntity<Player>(out playerEntity))
            return;

        DebugParabolaData debugInfo;
        if (!TryGetSingleton<DebugParabolaData>(out debugInfo))
             return;

        Entity boardEntity;
        if (!TryGetSingletonEntity<BoardSize>(out boardEntity))
            return;

        var boardSize = GetComponent<BoardSize>(boardEntity);
        var offsets = GetBuffer<OffsetList>(boardEntity);

        float currentTime = (float)Time.ElapsedTime;
        var targetPoint = GetComponent<Translation>(playerEntity);
        var timeOffset =  debugInfo.Duration / System.Math.Max((float)debugInfo.SampleCount, 1.0);

        Entities.
            WithAll<DebugParabolaSampleTag>()
            .ForEach((ref Translation translation, ref Arc arc, ref BallTrajectory destination, ref Time t, in DebugParabolaSampleTag debugSample) =>
            {
                translation.Value = new float3(0.0f, 0.0f, 0.0f);
                if (currentTime < t.EndTime)
                    return;

                t.StartTime = currentTime + (float)timeOffset * (float)debugSample.id;
                t.EndTime   = t.StartTime + debugInfo.Duration;

                float3 landingPos = new float3(0,0,0);
                TraceUtils.TraceArc(
                    new float3(0, 0, 0),
                    targetPoint.Value,
                    boardSize, offsets, out landingPos, out arc);

                destination.Source = translation.Value;
                destination.Destination = landingPos;
            }).Run();
    }
}
