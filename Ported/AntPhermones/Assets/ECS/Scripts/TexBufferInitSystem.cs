using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;


[UpdateInGroup(typeof(InitializationSystemGroup))]
public class TexBufferInitSystem : SystemBase
{

    EndSimulationEntityCommandBufferSystem cmdBufferSystem;

    protected override void OnCreate()
    {
        cmdBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        RequireSingletonForUpdate<TexInitialiser>();
    }

    protected override void OnUpdate()
    {
        //Get the singleton entities of the Texture Initaliser and Texture Data Store Entity
        Entity initEntity = GetSingletonEntity<TexInitialiser>();
        EntityManager.RemoveComponent<TexInitialiser>(initEntity);

        Entity texStoreEntity = EntityManager.CreateEntity();
        EntityManager.AddComponent<TexSingleton>(texStoreEntity);

        //Add a buffer to the texture store entity
        int TexSize = RefsAuthoring.TexSize;
        DynamicBuffer<PheromonesBufferData> pheromoneBuffer = EntityManager.AddBuffer<PheromonesBufferData>(texStoreEntity);

        pheromoneBuffer.EnsureCapacity(RefsAuthoring.TexSize * RefsAuthoring.TexSize); //Assure our capacity so we only expand size once
        pheromoneBuffer.Length = RefsAuthoring.TexSize * RefsAuthoring.TexSize;

        //Fill the buffer with 0's
        for (int i = 0; i < TexSize; ++i)
        {
            for (int j = 0; j < TexSize; ++j)
            {
                pheromoneBuffer[j * TexSize + i] = 0;
            }
        }
    }
}
