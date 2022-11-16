using System.ComponentModel.Design.Serialization;
using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Jobs;
// using UnityEngine;  
using Unity.Mathematics;
using Unity.Transforms;


[BurstCompile]
partial struct WallSpawnSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // question why am i not spawning the walls here
        // or using a job to spawn the walls? 
        
        state.RequireForUpdate<Config>();
    }
 
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
 
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        
        int WallRingCount = config.WallRingCount; // obstacle ring count 
        float WallPercentage = config.WallPercentage; // obstacles per ring 
        float WallRadius = config.WallRadius; // obstacle radius 
        int MapSize = config.MapSize; // mapSize 
        

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
        
        // do stuff 
        var rand = new Random(123); // if you want something random need to pass it in from delta.time 
        for (int i = 1; i <= WallRingCount; i++)
        {
            // operations for each ring 
            float ringRadius = (i / (WallRingCount + 1f)) * (MapSize * .5f);
            float circumference = ringRadius * 2f * math.PI;
            int maxCount = (int) math.ceil(circumference / (2f * WallRadius) * 2f);
            
            int offset = rand.NextInt(maxCount);
            int holeCount = rand.NextInt(1, 3); 
            
            // operations to instantiate each wall sphere
            for (int j = 0; j < maxCount; j++) {
                float t = (float) j / maxCount;
                if ((t * holeCount) % 1f < WallPercentage) {
                    float angle = (j + offset) / (float)maxCount * (2f * math.PI);

                    LocalToWorldTransform wallTransform = new LocalToWorldTransform();
                    wallTransform.Value.Position = new float3(MapSize * .5f + math.cos(angle) * ringRadius,
                        MapSize * .5f + math.sin(angle) * ringRadius, 0f);
                    wallTransform.Value.Scale = WallRadius;
                    
                    // draw the WALL 
                    var instance = ECB.Instantiate(config.WallPrefab); // ECB command doesnt happen immediately
                    ECB.SetComponent(instance, wallTransform); // sets "LocalToTransform" component 
                    
                    // create a wall tag
                  //  Wall wall = new Wall(); 
                  // ECB.AddComponent(instance, wall); // this adds "wall" as a tag to each entity
                  
                    ECB.AddComponent<Wall>(instance); // this line is equivalent to the above two lines 

                }
            }
        }

        state.Enabled = false;

    }
}