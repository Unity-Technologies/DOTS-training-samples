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
public class UpdatePheromoneSystem : SystemBase
{
    public static Color[] colors = new Color[RefsAuthoring.TexSize * RefsAuthoring.TexSize];

    public const float dt = 3.0f / 60;
    public const float decay = 0.9995f; // Original code used 0.9985f;
    public const float trailAddSpeed = 0.3f;
    public const float excitementWhenWandering = 0.3f;
    public const float excitementWithTargetInSight = 1.0f;
    public const float antSpeed = 0.2f;

    public static void DropPheromones(float x, float y, float2 bounds, NativeArray<PheromonesBufferData> localPheromones, float speed, float dt, int TexSize, float excitement)
    {
        float2 texelCoord = new float2(0.5f * (-x / bounds.x) + 0.5f, 0.5f * (-y / bounds.y) + 0.5f);
        int index = (int)(texelCoord.y * TexSize) * TexSize + (int)(texelCoord.x * TexSize);

        if (index >= localPheromones.Length || index < 0) return;

        excitement *= speed / antSpeed;

        var pheromone = (float)localPheromones[index];
        pheromone += (trailAddSpeed * excitement * dt) * (1f - pheromone);
        if (pheromone > 1f)
        {
            pheromone = 1f;
        }

        localPheromones[index] = pheromone;
    }

    protected override void OnUpdate()
    {
        int TexSize = RefsAuthoring.TexSize;

        //Get the pheremones data 
        EntityQuery doesTextInitExist = GetEntityQuery(typeof(TexInitialiser));
        if (!doesTextInitExist.IsEmpty)
        {
            return;
        }

        float2 bounds = AntMovementSystem.bounds;

        //AntMovementSystem.Depend.Complete();

        var texSingleton = GetSingletonEntity<TexSingleton>();

        Entities
        .ForEach((int entityInQueryIndex, ref Translation translation, ref Direction direction, ref RandState rand,
            in HasTargetInSight hasTargetInSight, in Speed speed) =>
        {
            var localPheromones = GetBuffer<PheromonesBufferData>(texSingleton);
            var excitement = hasTargetInSight.Value ? excitementWithTargetInSight : excitementWhenWandering;
            DropPheromones(translation.Value.x, translation.Value.z, bounds, localPheromones.AsNativeArray(), speed.Value, dt, TexSize, excitement);

        })
        .ScheduleParallel(JobHandle.CombineDependencies(AntMovementSystem.LastJob, this.Dependency));
        
        Entities
        .WithoutBurst()
        .ForEach((in AntSpawner dummy) =>
        {
            var localPheromones = GetBuffer<PheromonesBufferData>(texSingleton);
            for (int i = 0; i < TexSize; ++i)
            {
                for (int j = 0; j < TexSize; ++j)
                {
                    localPheromones[j * TexSize + i] *= decay;
                    colors[j * TexSize + i].r = localPheromones[j * TexSize + i];
                    colors[j * TexSize + i].g = 0;
                    colors[j * TexSize + i].b = 0;
                }
            }
        })
        .ScheduleParallel();
    }
}
