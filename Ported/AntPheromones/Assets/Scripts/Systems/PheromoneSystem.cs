using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using System;

//Singleton
public class PheromoneSystem : SystemBase
{
    private bool _initialized = false;
    [SerializeField] private int _bufferSize = 16384;

    protected override void OnUpdate()
    {
        // Don't use an init bool like this
        // Initialize in a separate system (component gets removed like in the tutorial)
        // Check to see if the singleton already exists. If it doesn't exist, create it
        if (!_initialized)
        {
            _initialized = true;

            Entity pheromoneEntity2 = EntityManager.CreateEntity(typeof(PheromoneStrength));

            DynamicBuffer<PheromoneStrength> pheromoneBuffer = GetBuffer<PheromoneStrength>(pheromoneEntity2);

            // If this is set to an amout lower than the original capacity, it will use the original capacity
            pheromoneBuffer.Capacity = _bufferSize;
            pheromoneBuffer.Length = _bufferSize;

            for (int i = 0; i < pheromoneBuffer.Length; i++)
            {
                pheromoneBuffer[i] = 50f;
            }
        }


    }
}
