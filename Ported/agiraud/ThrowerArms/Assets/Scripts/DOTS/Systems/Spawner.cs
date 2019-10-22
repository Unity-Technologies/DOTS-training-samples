using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public struct SpawnerData : IComponentData
{
    public Entity EntityPrefab;
    public int Count;
}

public class Spawner : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity e, ref SpawnerData spawnerData) =>
        {

            for (var i = 0; i < spawnerData.Count; i++)
            {
                var instance = EntityManager.Instantiate(spawnerData.EntityPrefab);

                // Place the instantiated in a grid with some noise
                var position = new float3(i * 1.3F, 5f, 15f);
                EntityManager.SetComponentData(instance, new Translation { Value = position });
            }

            EntityManager.DestroyEntity(e);
        });
    }
}
