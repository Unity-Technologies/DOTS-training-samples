using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class ScoreSystem : SystemBase
{
    private EntityQuery m_ReachedBaseQuery;

    private bool m_Initialised;

    private int m_NumPlayers;
    private NativeArray<int> m_Scores;

    private int m_MouseScoreAddition;
    private float m_CatScoreMultiplier;

    private EndSimulationEntityCommandBufferSystem m_EndSimulationECBS;

    EntityQuery m_BasesQuery;

    public List<int> GetWinningPlayers()
    {
        List<int> winningPlayers = new List<int>();

        int greatestScore = -1;
        for (int i = 0; i < m_NumPlayers; i++)
        {
            var playerScore = m_Scores[i];
            if (playerScore == greatestScore)
            {
                winningPlayers.Add(i);
            }
            else if (playerScore > greatestScore)
            {
                winningPlayers.Clear();
                winningPlayers.Add(i);
                greatestScore = playerScore;
            }
        }

        return winningPlayers;
    }

    protected override void OnCreate()
    {
        m_ReachedBaseQuery = GetEntityQuery(new EntityQueryDesc()
        {
            All = new[]
            {
                ComponentType.ReadOnly<ReachedBase>()
            }
        });

        m_EndSimulationECBS = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        m_BasesQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
           {
                ComponentType.ReadOnly<PlayerBase>()
            }
        });
    }

    protected override void OnDestroy()
    {
        if (m_Initialised)
        {
            m_NumPlayers = 0;
            m_Scores.Dispose();

            m_Initialised = false;
        }
    }

    bool Init()
    {
        if (m_Initialised)
            return true;

        var constantData = ConstantData.Instance;
        if (constantData == null)
            return false;

        m_NumPlayers = constantData.NumPlayers;
        m_Scores = new NativeArray<int>(m_NumPlayers, Allocator.Persistent);

        m_MouseScoreAddition = constantData.MouseScoreAddition;
        m_CatScoreMultiplier = constantData.CatScoreMultiplier;

        m_Initialised = true;
        return true;
    }

    protected override void OnUpdate()
    {
        if (!Init())
            return;

        var numPlayers = m_NumPlayers;
        var scores = m_Scores;

        var mouseAddition = m_MouseScoreAddition;
        var catMultiplier = m_CatScoreMultiplier;

        var playerBases = m_BasesQuery.ToComponentDataArrayAsync<PlayerBase>(Allocator.TempJob, out var playerBasesHandle);
        var playerBaseEntities = m_BasesQuery.ToEntityArrayAsync(Allocator.TempJob, out var playerBaseEntitiesHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, playerBasesHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, playerBaseEntitiesHandle);
        var ecb = m_EndSimulationECBS.CreateCommandBuffer();

        var baseAbsorbScale = ConstantData.Instance.BaseAbsorbScale;
        var baseAbsorbScaleTime = ConstantData.Instance.BaseAbsorbScaleTime;

        Entities
            .WithName("AddMiceScore")
            .WithAll<MouseTag>()
            .ForEach((Entity entity, in ReachedBase reachedBase) =>
            {
                var playerId = reachedBase.PlayerID;
                if (playerId >= 0 && playerId < numPlayers)
                {
                    scores[playerId] += mouseAddition;
                    for (int i = 0; i < numPlayers; i++)
                    {
                        if (playerBases[i].PlayerID == playerId)
                        {
                            ecb.AddComponent(playerBaseEntities[i], new ScaleRequest { Scale = baseAbsorbScale, Time = baseAbsorbScaleTime });
                            break;
                        }
                    }
                }
                ecb.DestroyEntity(entity);
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
                    scores[playerId] = (int)math.round(scores[playerId] * catMultiplier);
                    for (int i = 0; i < numPlayers; i++)
                    {
                        if (playerBases[i].PlayerID == playerId)
                        {
                            ecb.AddComponent(playerBaseEntities[i], new ScaleRequest { Scale = baseAbsorbScale, Time = baseAbsorbScaleTime });
                            break;
                        }
                    }
                }
                ecb.DestroyEntity(entity);

            })
            .Run();

        playerBases.Dispose();
        playerBaseEntities.Dispose();

        // Update Score UI
        var uiHelper = UIHelper.Instance;
        if (UIHelper.Instance != null)
        {
            for (int i = 0; i < m_Scores.Length; i++)
            {
                uiHelper.SetScore(i, m_Scores[i]);
            }
        }
    }
}