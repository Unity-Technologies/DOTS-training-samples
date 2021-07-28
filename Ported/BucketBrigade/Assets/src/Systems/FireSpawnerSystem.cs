using System;
using src.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
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
            var config = GetComponent<FireSimConfig>(configEntity);

            // Create temperature grid
            Entity temperatureEntity = EntityManager.CreateEntity(typeof(Temperature));

            DynamicBuffer<Temperature> buffer = EntityManager.GetBuffer<Temperature>(temperatureEntity);

            int totalCells = configValues.Columns * configValues.Rows;

            buffer.ResizeUninitialized(totalCells);

            for (int cell = 0; cell < totalCells; ++cell)
            {
                buffer[cell] = new Temperature
                {
                    Intensity = 0
                };
            }

            // Set starting fires
            for (int i=0; i<configValues.StartingFireCount; ++i)
            {
                buffer[Mathf.FloorToInt(UnityEngine.Random.Range(0, totalCells))] = new Temperature
                {
                    Intensity = UnityEngine.Random.Range(configValues.Flashpoint, 1f)
                };
            }

            // Create fire cell entities
            if (config.FireCellPrefab != null)
            {
                using var fireCellEntities = new NativeArray<Entity>(totalCells, Allocator.Temp);
                EntityManager.Instantiate(config.FireCellPrefab, fireCellEntities);

                for (var i = 0; i < fireCellEntities.Length; i++)
                {
                    var entity = fireCellEntities[i];
                    EntityManager.AddComponent<URPMaterialPropertyBaseColor>(entity);

                }
            }

            // Once we've spawned, we can disable this system, as it's done its job.
            Enabled = false;
        }
    }
}