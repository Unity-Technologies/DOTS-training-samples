using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Update the game camera
/// </summary>
[UpdateAfter(typeof(DestroySystem))]
public class GameWaitForRestartSystem : ComponentSystem
{
    EntityQuery m_Query;
    EntityQuery m_MapQuery;

    protected override void OnCreate()
    {
        m_Query = GetEntityQuery(typeof(LbGameRestarter));
        m_MapQuery = GetEntityQuery(typeof(LbMap));
    }

    /// <summary>
    /// Update the game
    /// </summary>
    protected override void OnUpdate()
    {
        var restartCompleted = false;
        Entities.ForEach((ref LbGameRestarter restarter) =>
        {
            restarter.Value -= Time.deltaTime;
            if (restarter.Value <= 0.0f)
            {
                restartCompleted = true;
            }
        });

        if (restartCompleted)
        {
            EntityManager.DestroyEntity(m_Query);
            EntityManager.DestroyEntity(m_MapQuery);

            Entities.ForEach((Entity entity, ref LbBoardGenerator generator) =>
            {
                EntityManager.RemoveComponent(entity, typeof(LbDisabled));
            });
        }
    }
}