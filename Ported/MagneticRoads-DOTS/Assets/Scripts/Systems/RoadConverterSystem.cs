using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class RoadConverterSystem : SystemBase
{
    protected override void OnUpdate()
    {
        if (!RoadGenerator.bullshit || RoadGenerator.trackSplines.Count == 0)
            return;
        
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var roadEntityBuffer = new Entity[RoadGenerator.trackSplines.Count*4];
        //TODO use a job for each spline
        foreach (var splineDef in RoadGenerator.subSplines)
        {
            roadEntityBuffer[splineDef.splineId] = CreateRoadEntity(ecb, splineDef);
        }

        var intersectionEntityBuffer = new Entity[RoadGenerator.originIntersectionCount*2];
        for (var i = 0; i < RoadGenerator.originIntersectionCount*2; i++)
        {
            var intersectionEntity = ecb.CreateEntity();
            ecb.AddBuffer<CarQueue>(intersectionEntity);

            ecb.AddComponent<Translation>(intersectionEntity);
            ecb.SetComponent(intersectionEntity, new Translation{Value = RoadGenerator.intersections[i/2].position});
            
            intersectionEntityBuffer[i] = intersectionEntity;
        }
        
        //Add singleton containing dynamic buffer of spline def, spline links, spline to road and spline to intersection
        var wayfinderSingleton = ecb.CreateEntity();
        var splineBuffer = ecb.AddBuffer<SplineDefArrayElement>(wayfinderSingleton);
        foreach (var splineDef in RoadGenerator.subSplines)
        {
            splineBuffer.Add(new SplineDefArrayElement {Value = splineDef});
        }
        
        var splineLinkBuffer = ecb.AddBuffer<SplineLink>(wayfinderSingleton);
        foreach (var link in RoadGenerator.splineLinks)
        {
            splineLinkBuffer.Add(new SplineLink {Value = LinkToInt2(link)});
        }
        
        var splineIdToRoadBuffer = ecb.AddBuffer<SplineIdToRoad>(wayfinderSingleton);
        foreach (var entity in roadEntityBuffer)
        {
            splineIdToRoadBuffer.Add(new SplineIdToRoad {Value = entity});
        }
        
        var splineToIntersection = ecb.AddBuffer<IntersectionArrayElement>(wayfinderSingleton);
        for (var i = 0; i < RoadGenerator.subSplines.Length; i++)
        {
            splineToIntersection.Add(new IntersectionArrayElement {Value = intersectionEntityBuffer[RoadGenerator.intersectionLinks[i]]});
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

        ecb.AddBuffer<CarQueue>(entity);
        ecb.AddComponent(entity, new CarQueueMaxLength
        {
            length = Mathf.Max(1, (int)(splineDef.measuredLength / .3f)) //TODO add car length to const comp
        });
        return entity;
    }
}

