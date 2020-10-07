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
        // read board information (used to limit entity bounds to the grid)
        Vector2Int boardDimensions = Vector2Int.zero;
        float yOffset = 0;
        // read Board properties (on main thread at the moment though - needs dependencies to run before board, as that gets destroyed.)
        Entities.ForEach((Entity e, in BoardSpawner boardSettings) =>
        {
            yOffset = 1.0f + boardSettings.RandomYOffset; // ensure agents aren't z-fighting with the board. (might need some terrain-following stuff instead if we want more fidelity)
            boardDimensions = new Vector2Int(boardSettings.SizeX, boardSettings.SizeZ);
        }).Run();

        //Debug.Log( "Dimensions: " + boardDimensions.x.ToString() + ", " + boardDimensions.y.ToString());
        
        Entities
            .WithStructuralChanges() // we will destroy ourselves at the end of the loop
            .ForEach((Entity agentSpawnerSettings, in AgentSpawner spawner, in Translation t) =>
        {
            int agentsPerTeam = (spawner.AgentLineLength * 2) + spawner.TeamScoopers + spawner.TeamThrowers;
            int len = spawner.TeamCount * agentsPerTeam;
            
            // reserve a list of entities to clone prefab data into.
            // NB - no need to use persistent storage, a) because this is not the backing store of the entities, and b) because this is a temp/local object (oops)
            NativeArray<Entity> clonedAgents = new NativeArray<Entity>(len, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

            // clone agents
            EntityManager.Instantiate(spawner.AgentPrefab, clonedAgents);
            
            int numFullBucketPassers = spawner.AgentLineLength;
            int numEmptyBucketPassers = spawner.AgentLineLength;

            int index = 0;
            // spawn teams
            for (int team = 0; team < spawner.TeamCount && index < len; ++team)
            {
                // could perhaps create these components via EntityManager.AddComponent version that takes an EntityQuery.
                
                // is it more efficient to add all MyAgent components in a loop, and then add the tags as separate loops,
                // or to add the MyAgent components as part of the tag loops?

                // create bucket collectors / fillers
                for (int scooper = 0; scooper < spawner.TeamScoopers; ++scooper)
                {
                    Entity agent = clonedAgents[index];
                    
                    EntityManager.AddComponentData<Agent>(agent, new Agent {TeamID = team});
                    EntityManager.AddComponent<AgentTags.ScooperTag>(agent);
                    
                    // place at random location within board
                    EntityManager.SetComponentData<Translation>(agent, new Translation(){Value = new float3(Random.Range(0,boardDimensions.x),yOffset,Random.Range(0, boardDimensions.y))});
                    
                    ++index;
                }
                
                // create throwers / extinguishers
                for (int thrower = 0; thrower < spawner.TeamThrowers; ++thrower)
                {
                    Entity agent = clonedAgents[index];
                    EntityManager.AddComponentData<Agent>(agent, new Agent {TeamID = team});
                    EntityManager.AddComponent<AgentTags.ThrowerTag>(agent);
                    
                    // place at random location within board
                    EntityManager.SetComponentData<Translation>(agent, new Translation(){Value = new float3(Random.Range(0,boardDimensions.x),yOffset,Random.Range(0, boardDimensions.y))});
                    ++index;
                }
                
                // create bucket-passing lines
                for (int fullPasser = 0; fullPasser < numFullBucketPassers; ++fullPasser)
                {
                    Entity agent = clonedAgents[index];
                    EntityManager.AddComponentData<Agent>(agent, new Agent {TeamID = team});
                    EntityManager.AddComponent<AgentTags.FullBucketPasserTag>(agent);

                    // place at random location within board
                    EntityManager.SetComponentData<Translation>(agent, new Translation(){Value = new float3(Random.Range(0,boardDimensions.x),yOffset,Random.Range(0, boardDimensions.y))});
                    ++index;
                }
                
                for (int emptyPasser = 0; emptyPasser < numEmptyBucketPassers; ++emptyPasser)
                {
                    Entity agent = clonedAgents[index];
                    EntityManager.AddComponentData<Agent>(agent, new Agent {TeamID = team});
                    EntityManager.AddComponent<AgentTags.EmptyBucketPasserTag>(agent);
                    
                    // place at random location within board
                    EntityManager.SetComponentData<Translation>(agent, new Translation(){Value = new float3(Random.Range(0,boardDimensions.x),yOffset,Random.Range(0, boardDimensions.y))});
                    ++index;    
                }
            }
            
            EntityManager.DestroyEntity(agentSpawnerSettings);
        }).Run();
    }
}
