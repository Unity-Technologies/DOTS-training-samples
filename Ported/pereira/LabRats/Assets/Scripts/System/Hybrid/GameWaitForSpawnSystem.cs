using Unity.Entities;
using UnityEngine;


public class GameWaitForSpawnSystem : ComponentSystem
{
    EntityQuery m_Query;

    protected override void OnCreate()
    {
        m_Query = GetEntityQuery(typeof(LbGameWaitForSpawn));
    }

    protected override void OnUpdate()
    {
        bool spawnCreated = false;

        Entities.ForEach((ref LbGameWaitForSpawn waiter) =>
        {
            waiter.Value -= Time.deltaTime;
            if (waiter.Value <= 0.0f)
            {
                var spawnAll = EntityManager.CreateEntity();
                EntityManager.AddComponentData(spawnAll, new LbGameSpawnAll());

                spawnCreated = true;
            }
        });

        if (spawnCreated)
        {
            EntityManager.DestroyEntity(m_Query);
        }
    }
}