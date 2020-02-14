using Unity.Burst;
using Unity.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

public class PathMoverSystem : JobComponentSystem
{
    private NativeArray<float3> PathData;
    protected override void OnCreate()
    {
        base.OnCreate();

        List<float3> testPoints = new List<float3>();

        for(int i = 0; i < 100; i++) 
        {
            testPoints.Add(new float3(i, 0, 0));
        }

        for (int i = 99; i >= 0; i--)
        {
            testPoints.Add(new float3(i, 0, 0));
        }

        PathData = new NativeArray<float3>(testPoints.ToArray(), Allocator.Persistent);
    }

    protected override void OnDestroy()
    {
        PathData.Dispose();
        base.OnDestroy();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var pathData = PathData;
        var dt = Time.DeltaTime;
        var totalTime = 0.5f;

        var outputDeps = Entities.ForEach((ref Translation translation, ref PathMoverComponent pathMoverComponent) =>
        {
            int2 range = pathMoverComponent.PathIndices;
            int nextPoint = (pathMoverComponent.CurrentPointIndex + 1) % (range.y - range.x);

            float3 currentPosition = pathData[pathMoverComponent.CurrentPointIndex + range.x];
            float3 nextPosition = pathData[nextPoint + range.x];

            translation.Value = math.lerp(currentPosition, nextPosition, pathMoverComponent.t);
            pathMoverComponent.t += dt / totalTime;

            if (pathMoverComponent.t > 1) 
            {
                pathMoverComponent.CurrentPointIndex = nextPoint;
                pathMoverComponent.t = 0;
            }

        }).Schedule(inputDeps);
        return outputDeps;
    }
}