
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
                randomComponent.Value.NextFloat(-numRow/2+1, numRow/2-1) ,
                0.1f,
                randomComponent.Value.NextFloat(-numCol/2+1, numCol/2-1));
            
            bucketTransform.Scale = 0.2f; //This is the scale of the bucket
            
            ECB.SetComponent(instance,bucketTransform);
            
        }
        
        
        
        //Disable state after spawning for now 
        state.Enabled = false;
    }
}
