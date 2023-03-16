
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public partial struct BucketSpawningSystem : ISystem
{
    private int numRow;
    private int numCol;
    private int totalBuckets;
    private Entity bucketPrefab;
    private Config config; 
    
    
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }


    public void OnUpdate(ref SystemState state)
    {
        // Get the config 
        config = SystemAPI.GetSingleton<Config>();
        
        //Get the total amount of buckets
        totalBuckets = config.totalBuckets;
        Debug.Log(totalBuckets);
        //Get the bucket prefab
        bucketPrefab = config.Bucket;
        
        //Get the rows and colums 
        numRow = config.rows;
        numCol = config.columns;
        
        //Get the random component 
        Random randomComponent = SystemAPI.GetSingleton<Random>();

        //Get the ECB
        var ECBSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ECB = ECBSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
        //Loop through and spawn at a random position
        for (int i = 0; i < totalBuckets; i++)
        {
            //Instatiate the bucket
            var instance = ECB.Instantiate(bucketPrefab);
            //Set its transform
            var bucketTransform = LocalTransform.FromPosition(
                randomComponent.Value.NextFloat(1, numRow-1) ,
                0.1f,
                randomComponent.Value.NextFloat(1, numCol-1));
            
            bucketTransform.Scale = 0.2f; //This is the scale of the bucket
            
            ECB.SetComponent(instance,bucketTransform);
            ECB.SetName(instance, new FixedString64Bytes("bucket_" + i));
            
            ECB.SetComponentEnabled<FillingTag>(instance, true); 
            ECB.SetComponentEnabled<EmptyTag>(instance, true); 
            ECB.SetComponentEnabled<FullTag>(instance, false);
        }
        
        
        
        //Disable state after spawning for now 
        state.Enabled = false;
    }
}
