using Miscellaneous.Execute;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = UnityEngine.Random;

public partial struct TeamBotSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;

        var config = SystemAPI.GetSingleton<Grid>();

        var botsPerTeam =
            config.NumTeambotPassTowardsWater + config.NumTeambotPassTowardsFire + 2; // (2 = 1 Douser, 1 WaterGatherer)
        var totalTeambotsCount = config.NumTeams * botsPerTeam;
        state.EntityManager.Instantiate(config.TeambotPrefab, totalTeambotsCount, Allocator.Temp);

        NativeArray<Entity> fireDousersArray = new NativeArray<Entity>(config.NumTeams, Allocator.Temp);
        NativeArray<Entity> waterGatherersArray = new NativeArray<Entity>(config.NumTeams, Allocator.Temp);

        // Assign Role and TeamID
        var teambotLookup = SystemAPI.GetComponentLookup<Teambot>();
        Entity firstTeammate = default;
        Entity prevTeambot = default;
        int i = 0;
        foreach (var (trans, teambot, teambotEntity) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Teambot>>()
                     .WithEntityAccess())
        {
            // Assign TeamID
            var teamID = (int)math.floor(i / botsPerTeam);
            teambot.ValueRW.TeamID = teamID;

            // Water Gatherer
            var botIndexInTeam = i % botsPerTeam;
            if (botIndexInTeam == 0)
            {
                teambot.ValueRW.Role = TeamBotRole.WaterGatherer;
                waterGatherersArray[teamID] = teambotEntity;
                firstTeammate = teambotEntity;
            }

            // Pass Towards Water
            else if (botIndexInTeam <= config.NumTeambotPassTowardsWater)
            {
                teambot.ValueRW.Role = TeamBotRole.PassTowardsWater;

                // set position in line
                teambot.ValueRW.PositionInLine = (float)botIndexInTeam / (config.NumTeambotPassTowardsWater + 1);
            }

            // Fire Douser
            else if (botIndexInTeam <= config.NumTeambotPassTowardsWater + 1)
            {
                teambot.ValueRW.Role = TeamBotRole.FireDouser;
                fireDousersArray[teamID] = teambotEntity;
            }

            // Pass Towards Fire
            else
            {
                teambot.ValueRW.Role = TeamBotRole.PassTowardsFire;
                teambot.ValueRW.PositionInLine = (float)(botIndexInTeam - config.NumTeambotPassTowardsWater - 1) / (config.NumTeambotPassTowardsWater + 1); 
            }

            // Assign pass to target
            if (botIndexInTeam != 0)
            {
                teambot.ValueRW.PassToTarget = prevTeambot;
                if (botIndexInTeam == botsPerTeam - 1)
                {
                    // Assign current entity to the first person in the team (water gatherer)
                    var firstTeammateInTeam = teambotLookup[firstTeammate];
                    firstTeammateInTeam.PassToTarget = teambotEntity;
                    teambotLookup[firstTeammate] = firstTeammateInTeam;
                }
            }

            // increment 
            prevTeambot = teambotEntity;
            i++;
        }

        // Assign Fire Dousers
        i = 0;
        foreach (var (trans, teambot, teambotEntity) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Teambot>>()
                     .WithEntityAccess())
        {
            // Assign TeamID
            var teamID = (int)math.floor(i / botsPerTeam);

            // Assign Leaders
            teambot.ValueRW.TeamFireDouser = fireDousersArray[teamID];
            teambot.ValueRW.TeamWaterGatherer = waterGatherersArray[teamID];

            // move to random position
            
            trans.ValueRW.Scale = 1;
            trans.ValueRW.Position.x = Random.Range(-30f,30f);
            trans.ValueRW.Position.y = 0;
            trans.ValueRW.Position.z = Random.Range(0,75f);
            
            
            // increment
            i++;
        }
    }
}