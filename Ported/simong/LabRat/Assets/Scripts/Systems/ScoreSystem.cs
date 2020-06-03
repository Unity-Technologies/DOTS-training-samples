using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ScoreSystem : SystemBase
{
    private EntityQuery m_ReachedBaseQuery;

    private int m_NumPlayers;
    private NativeArray<int> m_Scores;
    
    protected override void OnCreate()
    {
        m_ReachedBaseQuery = GetEntityQuery(new EntityQueryDesc()
        {
            All = new[]
            {
                ComponentType.ReadOnly<ReachedBase>()
            }
        });
        
        m_NumPlayers = ConstantData.Instance.NumPlayers;
        m_Scores = new NativeArray<int>(m_NumPlayers, Allocator.Persistent);
    }

    protected override void OnUpdate()
    {
        var numPlayers = m_NumPlayers;
        var scores = m_Scores;
        
        Entities
            .WithName("AddMiceScore")
            .WithAll<MouseTag>()
            .ForEach((Entity entity, in ReachedBase reachedBase) =>
            {
                var playerId = reachedBase.PlayerID;
                if (playerId >= 0 && playerId < numPlayers)
                {
                    scores[playerId] += 1;
                }
            })
            .Run();
        
        Entities
            .WithName("DeductCatScore")
            .WithAll<CatTag>()
            .ForEach((Entity entity, in ReachedBase reachedBase) =>
            {
                var playerId = reachedBase.PlayerID;
                if (playerId >= 0 && playerId < numPlayers)
                {
                    scores[playerId] = (int) math.round(scores[playerId] * 0.66f);
                }
            })
            .Run();
        
        EntityManager.DestroyEntity(m_ReachedBaseQuery);
    }
}