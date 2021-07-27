using System;
using src.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace src.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class FireSpawnerSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            RequireSingletonForUpdate<FireSimConfig>();
        }

        protected override void OnUpdate()
        {
            var configEntity = GetSingletonEntity<FireSimConfig>();

            var configValues = GetComponent<FireSimConfigValues>(configEntity);

            // Create temperature grid
            Entity temperatureEntity = EntityManager.CreateEntity(typeof(Temperature));

            DynamicBuffer<Temperature> buffer = EntityManager.GetBuffer<Temperature>(temperatureEntity);

            buffer.ResizeUninitialized(configValues.Columns * configValues.Rows);

            for (int y = 0; y < configValues.Rows; ++y)
            {
                for (int x = 0; x < configValues.Columns; ++x)
                {
                    buffer[x + y * configValues.Columns] = new Temperature
                    {
                        Intensity = 0
                    };
                }
            }
            
            // Once we've spawned, we can disable this system, as it's done its job.
            Enabled = false;
        }
    }
}