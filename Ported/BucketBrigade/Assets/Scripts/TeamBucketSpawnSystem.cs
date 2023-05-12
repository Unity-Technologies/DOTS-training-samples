using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(TeamBotSpawnerSystem))]
[BurstCompile]
public partial struct TeamBucketSpawnSystem : ISystem
{
    // [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Grid>();
        state.RequireForUpdate<Teambot>();
    }

    // [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;
        var config = SystemAPI.GetSingleton<Grid>();

        state.EntityManager.Instantiate(config.TeamBucketPrefab, config.NumTeams, state.WorldUpdateAllocator);

        Entity[] teambotArray = new Entity[config.NumTeams];
        var i = 0;
        foreach (var (teambot, teambotEntity) in SystemAPI.Query<RefRO<Teambot>>().WithEntityAccess())
        {
            if (teambot.ValueRO.Role == TeamBotRole.WaterGatherer)
            {
                teambotArray[i] = teambotEntity;
                i++;
            }
        } 

        i = 0;
        foreach (var teambucket in SystemAPI.Query<RefRW<TeamBucket>>())
        {
            teambucket.ValueRW.TargetTeambotEntity = teambotArray[i];
            i++;
        }


    }
}