using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;


public struct BrokenBar
{
    public Entity entity;
    public int index;
}

[BurstCompile]
struct BarSimulationJob : IJob
{
    public Entity entity;
    public BufferFromEntity<BarEntry> barAccessor;
    [NativeDisableContainerSafetyRestriction]
    public NativeArray<ConstrainedPoint> pointsBuf;
    public NativeList<BrokenBar> brokenBars;

    public float breakResistance;
    
    public void Execute()
    {
        var bars = barAccessor[entity];
        for (int i = 0; i < bars.Length; i++)
        {
            Bar bar = bars[i].Value;

            var point1 = pointsBuf[bar.p1];
            var point2 = pointsBuf[bar.p2];

            var dd = point2.position - point1.position;

            float dist = Unity.Mathematics.math.length(dd);
            float extraDist = dist - bar.length;

            var push = dd/dist * extraDist * 0.5f;

            if (point1.anchor == false && point2.anchor == false)
            {
                point1.position += push;
                point2.position -= push;
            }
            else if (point1.anchor)
            {
                point2.position -= push * 2f;
            }
            else if (point2.anchor)
            {
                point1.position += push * 2f;
            }

            pointsBuf[bar.p1] = point1;
            pointsBuf[bar.p2] = point2;

            if (Mathf.Abs(extraDist) > breakResistance)
            {
                brokenBars.Add(new BrokenBar {entity = entity, index = i});
            }
        }
    }
}


[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(TornadoSystem))]
public class BarSimulationSystem : JobComponentSystem
{
    EntityQuery q;
    EntityQuery generationSettings;
    
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<GenerationSystem.State>();
        q = GetEntityQuery(typeof(ConstrainedPointEntry));
        generationSettings = GetEntityQuery(typeof(GenerationSetting));
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var settingsEntity = generationSettings.GetSingletonEntity();
        var settings = EntityManager.GetComponentObject<GenerationSetting>(settingsEntity);

        var e = q.GetSingletonEntity();
        var pointsBuf = EntityManager.GetBuffer<ConstrainedPointEntry>(e);

        var entities = GetEntityQuery(typeof(BarEntry)).ToEntityArray(Allocator.TempJob);
        var jobs = new NativeArray<JobHandle>(entities.Length, Allocator.TempJob);
        var breakage = new NativeList<BrokenBar>[entities.Length];
        
        var barAccessor = GetBufferFromEntity<BarEntry>(isReadOnly: true);
        

        var localPointBuf = pointsBuf.AsNativeArray().Reinterpret<ConstrainedPoint>();
        
        for(int i = 0; i < entities.Length; ++i)
        {
            var brokenBars = new NativeList<BrokenBar>(Allocator.TempJob);

            breakage[i] = brokenBars;
            
            var job = new BarSimulationJob()
            {
                entity = entities[i],
                barAccessor = barAccessor,
                //bars = EntityManager.GetBuffer<BarEntry>(entities[i]),
                pointsBuf = localPointBuf,
                breakResistance = settings.breakResistance,
                brokenBars = brokenBars,
            };

            var jobHandle = job.Schedule();
            jobs[i] = jobHandle;
        }
        
        var jj = JobHandle.CombineDependencies(jobs);
        jj.Complete();
        
        
        barAccessor = GetBufferFromEntity<BarEntry>(isReadOnly: false);
        
        
        
        for (var i = 0; i < entities.Length; ++i)
        {
            if (breakage[i].Length > 0)
            {
                var bars = barAccessor[entities[i]];
                for (var j = 0; j < breakage[i].Length; j++)
                {
                    var currentBar = bars[breakage[i][j].index];
                    var point1 = pointsBuf[currentBar.Value.p1].Value;
                    var point2 = pointsBuf[currentBar.Value.p2].Value;
                    
                    if (point2.neighborCount > 1)
                    {
                        point2.neighborCount--;
                        var newPoint = new ConstrainedPoint();
                        newPoint = point2;
                        newPoint.neighborCount = 1;
                        newPoint.anchor = false;
                        pointsBuf.Add(new ConstrainedPointEntry { Value = newPoint });
                        currentBar.Value.p2 = pointsBuf.Length - 1;
                    }
                    else if (point1.neighborCount > 1)
                    {
                        point1.neighborCount--;
                        var newPoint = new ConstrainedPoint();
                        newPoint = point1;
                        newPoint.neighborCount = 1;
                        newPoint.anchor = false;
                        pointsBuf.Add(new ConstrainedPointEntry { Value = newPoint });
                        currentBar.Value.p1 = pointsBuf.Length - 1;
                    }
                    
                    bars[breakage[i][j].index] = currentBar;
                }
            }
            
            breakage[i].Dispose();
        }
        
        entities.Dispose();
        jobs.Dispose();
        return inputDeps;
    }
}
