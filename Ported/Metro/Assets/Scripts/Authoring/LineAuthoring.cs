using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class LineAuthoring : MonoBehaviour
{
    public List<GameObject> Platforms;
    public Color LineColor;

    class Baker : Baker<LineAuthoring>
    {
        public override void Bake(LineAuthoring authoring)
        {
            AddComponent(new Line()
            {
                Platforms = authoring.Platforms.ConvertAll(x => GetEntity(x)).ToNativeList(Allocator.Persistent),
                LineColor = (Vector4)authoring.LineColor
            });
        }
    }
}

struct Line : IComponentData
{
    public float4 LineColor;
    public NativeList<Entity> Platforms;
}
