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
            var stationBuffer = AddBuffer<PlatformEntity>();
            stationBuffer.EnsureCapacity(authoring.Platforms.Count);
            foreach (PlatformAuthoring authoringPlatform in authoring.Platforms)
            {
                stationBuffer.Add(new()
                {
                    Station = GetEntity(authoringPlatform.gameObject),
                    StopPos = authoringPlatform.TrainStopPosition
                });
            }

            AddComponent(new Line()
            {
                Entity = GetEntity(authoring),
                LineColor = (Vector4)authoring.LineColor
            });
        }
    }
}

public struct Line : IComponentData
{
    public Entity Entity;
    public float4 LineColor;
    public int Id;
}

public struct PlatformEntity : IBufferElementData
{
    public Entity Station;
    public float3 StopPos;
}
