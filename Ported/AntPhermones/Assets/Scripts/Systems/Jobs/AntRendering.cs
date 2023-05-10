using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[WithAll(typeof(Ant))]
public partial struct AntRenderingJob : IJobEntity
{
    public float mapSize;
    
    [BurstCompile]
    public void Execute(in Ant ant, ref URPMaterialPropertyBaseColor color)
    {
        if (ant.hasResource)
            color.Value = new Vector4(1, 1, 0, 1);
        else
            color.Value = new Vector4(0, 0, 1, 1);
    }
}