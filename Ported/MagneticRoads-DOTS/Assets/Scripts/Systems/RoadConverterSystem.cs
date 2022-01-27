using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public partial class RoadConverterSystem : SystemBase
{
    protected override void OnUpdate()
    {
        if (!RoadGenerator.bullshit || RoadGenerator.trackSplines.Count == 0)
            return;
        
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var entityBuffer = new Entity[RoadGenerator.trackSplines.Count*4];
        //TODO use a job for each spline
        foreach (var splineDef in RoadGenerator.subSplines)
        {
            entityBuffer[splineDef.splineId] = CreateRoadEntity(ecb, splineDef);
        }
        
        //Add singleton containing dynamic buffer of spline def and spline links
        var splineHolder = ecb.CreateEntity();
        var splineBuffer = ecb.AddBuffer<SplineDefArrayElement>(splineHolder);
        foreach (var splineDef in RoadGenerator.subSplines)
        {
            splineBuffer.Add(new SplineDefArrayElement {Value = splineDef});
        }
        
        var splineLinkBuffer = ecb.AddBuffer<SplineLink>(splineHolder);
        foreach (var link in RoadGenerator.splineLinks)
        {
            splineLinkBuffer.Add(new SplineLink {Value = LinkToInt2(link)});
        }
        
        var splineIdToRoadBuffer = ecb.AddBuffer<SplineIdToRoad>(splineHolder);
        foreach (var entity in entityBuffer)
        {
            splineIdToRoadBuffer.Add(new SplineIdToRoad {Value = entity});
        }
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
        
        Enabled = false;

        World.GetOrCreateSystem<CarSpawnerSystem>().Enabled = true;
    }

    private int2 LinkToInt2(List<int> link)
    {
        return link.Count == 1 ? new int2(link[0], int.MinValue) : new int2(link[0], link[1]);
    }
    
    private Entity CreateRoadEntity(EntityCommandBuffer ecb, SplineDef splineDef)
    {
        var entity = ecb.CreateEntity();

        ecb.AddComponent<CarQueueMaxLength>(entity);
        ecb.AddBuffer<CarQueue>(entity);
        ecb.AddComponent(entity, new RoadLength
        {
            roadLength = (int)(splineDef.measuredLength / 0.5f) //TODO add car length to const comp
        });
        return entity;
    }
}

