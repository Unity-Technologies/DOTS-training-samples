using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(AntMovementSystem))]
[AlwaysUpdateSystem]
public class UpdatePheromoneSystem : SystemBase
{
    public static Color[] colors = new Color[RefsAuthoring.TexSize * RefsAuthoring.TexSize];

    public const float dt = 3.0f / 60;
    
    public const float trailAddSpeed = 0.3f;
    public const float excitementWhenWandering = 0.3f;
    public const float excitementWithTargetInSight = 1.0f;
    public const float antSpeed = 0.2f;

    protected override void OnUpdate()
    {
        //Get the pheremones data 
        /*EntityQuery doesTextInitExist = GetEntityQuery(typeof(TexInitialiser));
        if (!doesTextInitExist.IsEmpty)
        {
            return;
        }

        if (!AntMovementSystem.PheromoneBuffer.IsCreated) return;*/

        /*var decayPass = new PheromoneDecayPassJob();
        decayPass.localPheromones = AntMovementSystem.PheromoneBuffer;

        LastJob = decayPass.Schedule(decayPass.localPheromones.Length, 128, AntMovementSystem.LastJob);*/
    }
}
