using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class LineAuthoring : MonoBehaviour
{
    public List<PlatformAuthoring> Platforms;
    public Color LineColor;

    class Baker : Baker<LineAuthoring>
    {
        public override void Bake(LineAuthoring authoring)
        {
            NativeList<float3> platformStopPos = new NativeList<float3>(authoring.Platforms.Count, Allocator.Persistent);
            NativeList<Entity> platformEnts = new NativeList<Entity>(authoring.Platforms.Count, Allocator.Persistent);
            foreach (PlatformAuthoring authoringPlatform in authoring.Platforms)
            {
                platformStopPos.Add(authoringPlatform.TrainStopPosition);
                platformEnts.Add(GetEntity(authoringPlatform.gameObject));
            }

            AddComponent(new Line()
            {
                platformStopPositions = platformStopPos,
                platforms = platformEnts,
                LineColor = (Vector4)authoring.LineColor
            });
        }
    }
}

public struct Line : IComponentData
{
    public float4 LineColor;
    public NativeList<float3> platformStopPositions;
    public NativeList<Entity> platforms;
    public int Id;
}
