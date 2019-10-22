using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class ThrowerArmsGroupSystem : ComponentSystemGroup { }

public struct SpawnerData : IComponentData
{
    public Entity EntityPrefab;
    public int Count;
}

[UpdateInGroup(typeof(ThrowerArmsGroupSystem))]
public class SpawnerSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity e, ref SpawnerData spawnerData) =>
        {
            for (var i = 0; i < spawnerData.Count; i++)
            {
                EntityManager.Instantiate(spawnerData.EntityPrefab);
            }
            EntityManager.DestroyEntity(e);
        });
    }
}
