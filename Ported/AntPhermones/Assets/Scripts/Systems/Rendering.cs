using NUnit.Framework.Internal;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public partial struct Rendering: ISystem 
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Ant>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var job = new AntRenderingJob();
        state.Dependency = job.Schedule(state.Dependency);
    }
}
