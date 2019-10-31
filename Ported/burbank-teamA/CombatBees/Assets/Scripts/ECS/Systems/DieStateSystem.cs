using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public class DieStateSystem : JobComponentSystem
{
    EndSimulationEntityCommandBufferSystem buffer;

    protected override void OnCreate()
    {
        buffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)

    {

        var commonBuffer = buffer.CreateCommandBuffer().ToConcurrent();
        var bloodPrefab = GetSingleton<BloodParticlesPrefab>().Value;

        var handle = Entities.WithBurst()
            .ForEach((Entity entity, in Translation translation, in State state) =>
        {
            if (state.Value == State.StateType.Dead)
            {

                var spawnedBlood = commonBuffer.Instantiate(0, bloodPrefab);
                commonBuffer.DestroyEntity(0, entity);
                commonBuffer.SetComponent(0, spawnedBlood, new Translation
                {
                    Value = translation.Value
                });

            }
        }).Schedule(inputDependencies);
        buffer.AddJobHandleForProducer(handle);
        return handle;

    }
}


// Input
// State


// Output
// State
// Spawner(Entity)
