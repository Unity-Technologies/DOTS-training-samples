using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[AlwaysUpdateSystem]
public class ScoreSystem : ComponentSystem
{
    private EntityQuery m_RatQuery;
    private EntityQuery m_CatQuery;

    /// <summary>
    /// Create all queries
    /// </summary>
    protected override void OnCreate()
    {
        m_RatQuery = GetEntityQuery(typeof(LbRatScore));
        m_CatQuery = GetEntityQuery(typeof(LbCatScore));
    }

    /// <summary>
    /// Update the game
    /// </summary>
    protected override void OnUpdate()
    {
        var ratScores = m_RatQuery.ToComponentDataArray<LbRatScore>(Allocator.TempJob);
        var catScores = m_CatQuery.ToComponentDataArray<LbCatScore>(Allocator.TempJob);

        Entities.ForEach((ref LbPlayer player, ref LbPlayerScore score) =>
        {
            // Add all Rat scores
            for (int i=0; i<ratScores.Length; ++i)
            {
                if (player.Value == ratScores[i].Player)
                {
                    score.Value += 1;
                }
            }

            // Remove all Cat scores
            for (int i=0; i<catScores.Length; ++i)
            {
                if (player.Value == catScores[i].Player)
                {
                    score.Value = (int)(score.Value * 0.6666f);
                }
            }
        });

        ratScores.Dispose();
        catScores.Dispose();

        EntityManager.DestroyEntity(m_RatQuery);
        EntityManager.DestroyEntity(m_CatQuery);
    }
}