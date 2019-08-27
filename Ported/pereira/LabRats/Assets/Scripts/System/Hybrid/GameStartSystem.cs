using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Update the game camera
/// </summary>
public class GameStartSystem : ComponentSystem
{
    GameIntro m_UIIntro;
    EntityQuery m_Query;

    protected override void OnCreate()
    {
        m_Query = GetEntityQuery(typeof(LbGameStart));
    }

    /// <summary>
    /// Update the game
    /// </summary>
    protected override void OnUpdate()
    {
        if (m_UIIntro == null)
        {
            m_UIIntro = GameObject.FindObjectOfType<GameIntro>();
            if (m_UIIntro == null)
                return;
        }

        m_UIIntro.Play(LbConstants.IntroTime);
        EntityManager.DestroyEntity(m_Query);

        var entity = EntityManager.CreateEntity();
        EntityManager.AddComponentData(entity, new LbGameWaitForSpawn() { Value = LbConstants.IntroTime });
    }
}