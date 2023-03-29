
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(BucketSpawningSystem))]
public partial struct BotSpawningSystem : ISystem
{
    private int numRow;
    private int numCol;
    private int totalBots;
    private int totalOmniworkers;
    private Entity botPrefab;
    private Entity omniworkerPrefab;
    private Config config; 
    
    
    public void OnUpdate(ref SystemState state)
    {
        config = SystemAPI.GetSingleton<Config>();
        totalBots = config.TotalBots;
        totalOmniworkers = config.TotalOmniworkers;
        botPrefab = config.Bot;
        omniworkerPrefab = config.Omniworker;
        numRow = config.rows;
        numCol = config.columns;
        
        Random randomComponent = SystemAPI.GetSingleton<Random>();
        //randomComponent.Value.InitState(4);
        
        var ECBSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ECB = ECBSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
        //Loop through and spawn bots at a random position
        for (int i = 0; i < totalBots; i++)
        {
            Debug.Log(totalBots);
            //Instatiate the bot
            var instance = ECB.Instantiate(botPrefab);
            //Set its transform
            var botTransform = LocalTransform.FromPosition(
                randomComponent.Value.NextFloat(-0.1f, numCol*config.cellSize + 0.1f) ,
                config.botStartingYPosition,
                randomComponent.Value.NextFloat(-0.1f, numRow*config.cellSize + 0.1f));
            
            botTransform.Scale = 1f; //This is the scale of the bot pls change this
            ECB.SetComponent(instance,botTransform);
            if (i != 0) //If it is not the first one
            {
                ECB.SetComponentEnabled<FrontBotTag>(instance, false); 
            } 
            if (i != totalBots - 1)//If it is not the last one
            {
                ECB.SetComponentEnabled<BackBotTag>(instance, false);
            }
            if (i < totalBots / 2 || i ==  totalBots - 1)//If it is part of the first half
            {
                ECB.SetComponentEnabled<BackwardPassingBotTag>(instance, false);
            }
            if (i >= totalBots / 2 || i == 0)//If it is part of the last half
            {
                ECB.SetComponentEnabled<ForwardPassingBotTag>(instance, false); 
            }
        }
        
        //Loop for omniworkers
        for (int i = 0; i < totalOmniworkers; i++)
        {
            var instance = ECB.Instantiate(omniworkerPrefab);
            //Set its transform
            var botTransform = LocalTransform.FromPosition(
                randomComponent.Value.NextFloat(-0.1f, numCol*config.cellSize + 0.1f) ,
                0.5f,
                randomComponent.Value.NextFloat(-0.1f, numRow*config.cellSize + 0.1f));
            
            botTransform.Scale = 1f;
            ECB.SetComponent(instance,botTransform);
            ECB.SetComponentEnabled<OmniworkerGoForBucketTag>(instance, true); 
            ECB.SetComponentEnabled<OmniworkerGoForWaterTag>(instance, false);
            ECB.SetComponentEnabled<OmniworkerGoForFireTag>(instance, false);
        }
        
        //Disable state after spawning for now 
        state.Enabled = false;
    }
}
