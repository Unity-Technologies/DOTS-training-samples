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

                float a, b, c;
                ParabolaUtil.CreateParabolaOverPoint(0.0f, 0.2f, 8.0f, targetPoint.Value.y, out a, out b, out c);
                arc.Value.x = a;
                arc.Value.y = b;
                arc.Value.z = c;

                destination.Source = translation.Value;
                destination.Destination = targetPoint.Value;
            }).Run();
    }
}
