using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = UnityEngine.Random;

//*
// BoardSpawnerSystem removes the entity that has a BoardSpawner component after it initializes itself.
// Update before that system to ensure we can read the board settings before then.
[UpdateBefore(typeof(BoardSpawnerSystem))]
/*/
 // for testing / validation purposes. 
 // force this system to update after BoardSpawner (by default it updates before BoardSpawner, so this is needed)
[UpdateAfter(typeof(BoardSpawnerSystem))] 
/**/
public class AgentSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Vector2Int boardDimensions = Vector2Int.zero;
        float boardYOffset = 0.0f;
       
        // read Board properties (on main thread at the moment though - needs dependencies to run before board, as that gets destroyed.)
        // also, this is pointless as a loop - the code doesn't support multiple boards.
        Entities.ForEach((Entity e, in BoardSpawner boardSettings) =>
        {
            boardYOffset = boardSettings.RandomYOffset; // ensure agents aren't z-fighting with the board. (might need some terrain-following stuff instead if we want more fidelity)
            boardDimensions = new Vector2Int(boardSettings.SizeX, boardSettings.SizeZ);
        }).Run();
        
        Entities
            .WithStructuralChanges() // we will destroy ourselves at the end of the loop
            .ForEach((Entity agentSpawnerSettings, in AgentSpawner spawner, in Translation t) =>
        {
            float maxAgentVelocity = spawner.MaxAgentVelocity;
            
            // uniform scale
            //Scale prefabScale = EntityManager.GetComponentData<Scale>(spawner.AgentPrefab);
            NonUniformScale prefabScale = EntityManager.GetComponentData<NonUniformScale>(spawner.AgentPrefab);
            float yOffset = prefabScale.Value.y * 0.5f + boardYOffset;
                    
            int agentsPerTeam = (spawner.AgentLineLength * 2) + spawner.TeamScoopers + spawner.TeamThrowers;
            int len = spawner.TeamCount * agentsPerTeam;
            
            // reserve a list of entities to clone prefab data into.
            // NB - no need to use persistent storage, a) because this is not the backing store of the entities, and b) because this is a local object (oops)
            NativeArray<Entity> clonedAgents = new NativeArray<Entity>(len, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

            // clone agents
            EntityManager.Instantiate(spawner.AgentPrefab, clonedAgents);
            
            int numFullBucketPassers = spawner.AgentLineLength;
            int numEmptyBucketPassers = spawner.AgentLineLength;

            int index = 0;
            // spawn teams
            for (int team = 0; team < spawner.TeamCount && index < len; ++team)
            {
                var teamEntity = EntityManager.CreateEntity(typeof(Team));
                EntityManager.SetName(teamEntity, $"Team {team}");
                var teamComponent = new Team { Id = team, Length = spawner.AgentLineLength};
            
                // could perhaps create these components via EntityManager.AddComponent version that takes an EntityQuery.
                
                // is it more efficient to add all MyAgent components in a loop, and then add the tags as separate loops,
                // or to add the MyAgent components as part of the tag loops?

                // create bucket collectors / fillers
                for (int scooper = 0; scooper < spawner.TeamScoopers; ++scooper)
                {
                    Entity agent = clonedAgents[index];
                    
                    EntityManager.AddComponentData<Agent>(agent, new Agent {Team = teamEntity, MaxVelocity = maxAgentVelocity, CarriedEntity = Entity.Null, ActionState = 0});
                    EntityManager.AddComponent<AgentTags.ScooperTag>(agent);

                    float3 spawnPos = new float3(Random.Range(0, boardDimensions.x), yOffset, Random.Range(0, boardDimensions.y));
                    //EntityManager.AddComponentData<SeekPosition>(agent, new TargetPosition{ MaxVelocity = 0.2f, TargetPos = new float3(spawnPos.x, spawnPos.y, spawnPos.z) });
                    EntityManager.AddComponentData<SeekPosition>(agent, new SeekPosition{ Velocity = 0, TargetPos = new float3(0,0,0) });
                    
                    // place at random location within board
                    EntityManager.SetComponentData<Translation>(agent, new Translation(){ Value = spawnPos });


                    EntityManager.SetComponentData<Color>(agent, new Color {Value = new float4(0, 1, 0, 1)});
                    ++index;
                }
                
                // create throwers / extinguishers
                for (int thrower = 0; thrower < spawner.TeamThrowers; ++thrower)
                {
                    Entity agent = clonedAgents[index];
                    EntityManager.AddComponentData<Agent>(agent, new Agent {Team = teamEntity, MaxVelocity = maxAgentVelocity, CarriedEntity = Entity.Null, ActionState = 0});
                    EntityManager.AddComponent<AgentTags.ThrowerTag>(agent);
                    
                    float3 spawnPos = new float3(Random.Range(0, boardDimensions.x), yOffset, Random.Range(0, boardDimensions.y));
                    //EntityManager.AddComponentData<SeekPosition>(agent, new TargetPosition{ MaxVelocity = 0.075f, TargetPos = new float3(spawnPos.x, spawnPos.y, spawnPos.z) });
                    EntityManager.AddComponentData<SeekPosition>(agent, new SeekPosition{ Velocity = 0, TargetPos = new float3(20,0,20) });
                    
                    // place at random location within board
                    EntityManager.SetComponentData<Translation>(agent, new Translation(){ Value = spawnPos });
                    
                    EntityManager.SetComponentData<Color>(agent, new Color {Value = new float4(1, 0, 1, 1)});
                    ++index;
                }
                
                Entity previous = Entity.Null;
                // create bucket-passing lines
                for (int fullPasser = 0; fullPasser < numFullBucketPassers; ++fullPasser)
                {
                    Entity agent = clonedAgents[index];
                    EntityManager.AddComponentData<Agent>(agent, new Agent {Team = teamEntity, MaxVelocity = maxAgentVelocity, CarriedEntity = Entity.Null, ActionState = 0, PreviousAgent = previous, NextAgent = Entity.Null});
                    EntityManager.AddComponent<AgentTags.FullBucketPasserTag>(agent);

                    float3 spawnPos = new float3(Random.Range(0, boardDimensions.x), yOffset, Random.Range(0, boardDimensions.y));
                    EntityManager.AddComponentData<SeekPosition>(agent, new SeekPosition{ Velocity = 0, TargetPos = new float3(Random.Range(0,20),0,Random.Range(0,20))  });
                    // place at random location within board
                    EntityManager.SetComponentData<Translation>(agent, new Translation(){ Value = spawnPos });
                    EntityManager.SetComponentData<Color>(agent, new Color {Value = new float4(1, 1, 0, 1)});
                    if (previous != Entity.Null)
                    {
                        var previousAgent = EntityManager.GetComponentData<Agent>(previous);
                        previousAgent.NextAgent = agent;
                        EntityManager.SetComponentData(previous, previousAgent);
                    }
                    else
                    {
                        teamComponent.LineFullHead = agent;
                    }
                    previous = agent;

                    ++index;
                }
                teamComponent.LineFullTail = previous;
                
                previous = Entity.Null;
                for (int emptyPasser = 0; emptyPasser < numEmptyBucketPassers; ++emptyPasser)
                {
                    Entity agent = clonedAgents[index];
                    EntityManager.AddComponentData<Agent>(agent, new Agent {Team = teamEntity, MaxVelocity = maxAgentVelocity, CarriedEntity = Entity.Null, ActionState = 0, PreviousAgent = previous, NextAgent = Entity.Null});
                    EntityManager.AddComponent<AgentTags.EmptyBucketPasserTag>(agent);
                    
                    float3 spawnPos = new float3(Random.Range(0, boardDimensions.x), yOffset, Random.Range(0, boardDimensions.y));
                    EntityManager.AddComponentData<SeekPosition>(agent, new SeekPosition{ Velocity = 0f, TargetPos = new float3(Random.Range(0,20),0,Random.Range(0,20)) });
                    // place at random location within board
                    EntityManager.SetComponentData<Translation>(agent, new Translation(){ Value = spawnPos });
                    EntityManager.SetComponentData<Color>(agent, new Color {Value = new float4(0, 1, 1, 1)});
                    if (previous != Entity.Null)
                    {
                        var previousAgent = EntityManager.GetComponentData<Agent>(previous);
                        previousAgent.NextAgent = agent;
                        EntityManager.SetComponentData(previous, previousAgent);
                    }
                    else
                    {
                        teamComponent.LineEmptyHead = agent;
                    }
                    previous = agent;
                   
                    ++index;    
                }
                teamComponent.LineEmptyTail = previous;
                
                EntityManager.AddComponentData(teamEntity, teamComponent);
            }
            
            EntityManager.DestroyEntity(agentSpawnerSettings);
        }).Run();
    }
}
