using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public struct AntOutput
{
    public int pos;
    public float value;
}

public struct AntOutputJob : IJobParallelFor
{
    public AntSettings Settings;
    [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Translation> Positions;
    [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<AntComponent> Ants;
    public NativeArray<AntOutput> Output;
    public float TimeDelta;

    public void Execute(int index)
    {
        AntOutput output = new AntOutput();

        var ant = Ants[index];
        var pos = Positions[index].Value;
        output.pos = (int)(pos.x * Settings.mapSize);

        bool holdingResource = true;
        float strength = (holdingResource ? 1.0f : .3f) * ant.speed / Settings.antSpeed;

        output.value = 10 * math.min(Settings.trailAddSpeed * strength * TimeDelta, 1.0f);
        Output[index] = output;
    }
}
