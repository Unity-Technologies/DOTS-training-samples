using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using System.Diagnostics;
using Unity.Rendering;
using Unity.Mathematics;
using System;
using Random = Unity.Mathematics.Random;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using Unity.Collections;
using UnityEditor.PackageManager;

[BurstCompile]
public partial struct GridTestSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<WorldGrid>();
        state.RequireForUpdate<Config>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        var worldGrid = SystemAPI.GetSingleton<WorldGrid>();

        FarmerAspect f = new FarmerAspect();
        foreach(var farmer in SystemAPI.Query<FarmerAspect>())
        {
            f = farmer;
        }

        int i = 0;
        foreach(var testPos in SystemAPI.Query<TransformAspect>().WithAll<GridTest>())
        {
            int2 gridPos = worldGrid.WorldToGrid(f.Transform.WorldPosition);
            float3 worldPos = worldGrid.GridToWorld(gridPos.x, gridPos.y);
            testPos.WorldPosition = worldPos;
            i++;
        }

    }

}

