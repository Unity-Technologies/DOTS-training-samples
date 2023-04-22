
using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;
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
    private int numTeams;

    public void OnUpdate(ref SystemState state)
    {
        config = SystemAPI.GetSingleton<Config>();
        totalBots = config.TotalBots;
        totalOmniworkers = config.TotalOmniworkers;

        botPrefab = config.Bot;
        omniworkerPrefab = config.Omniworker;
        numRow = config.rows;
        numCol = config.columns;
        numTeams = config.TotalTeams;

        Random randomComponent = SystemAPI.GetSingleton<Random>();

        //randomComponent.Value.InitState(4);

        var ECBSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ECB = ECBSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        //Colors (so it is color coded)
        URPMaterialPropertyBaseColor ForwardColor()
        {
            var color = UnityEngine.Color.magenta;
            return new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)color };
        }

        URPMaterialPropertyBaseColor BackwardColor()
        {
            var color = UnityEngine.Color.cyan;
            return new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)color };
        }

        URPMaterialPropertyBaseColor FrontColor()
        {
            var color = UnityEngine.Color.red;
            return new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)color };
        }

        URPMaterialPropertyBaseColor BackColor()
        {
            var color = UnityEngine.Color.blue;
            return new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)color };
        }



        for (int t = 0; t < numTeams; t++)
        {
            
            //Loop through and spawn bots at a random position
            for (int i = 0; i < totalBots; i++)
            {
                //Instatiate the bot
                var instance = ECB.Instantiate(botPrefab);
                //Set its transform
                var botTransform = LocalTransform.FromPosition(
                    randomComponent.Value.NextFloat(-0.1f, numCol * config.cellSize + 0.1f),
                    config.botStartingYPosition,
                    randomComponent.Value.NextFloat(-0.1f, numRow * config.cellSize + 0.1f));

                botTransform.Scale = 1f; //This is the scale of the bot pls change this
                ECB.SetComponent(instance, botTransform);

                if (i < totalBots / 2 || i == totalBots - 1) //If it is part of the first half
                {
                    ECB.SetComponentEnabled<BackwardPassingBotTag>(instance, false);
                    ECB.SetComponent(instance, ForwardColor());
                }

                if (i >= totalBots / 2 || i == 0) //If it is part of the last half
                {
                    ECB.SetComponentEnabled<ForwardPassingBotTag>(instance, false);
                    ECB.SetComponent(instance, BackwardColor());
                }

                if (i != 0) //If it is not the first one
                {
                    ECB.SetComponentEnabled<FrontBotTag>(instance, false);
                }
                else
                {
                    ECB.SetComponent(instance, FrontColor());
                    ECB.AddComponent<TeamReadyTag>(instance);
                    ECB.SetComponentEnabled<TeamReadyTag>(instance, false);
                }

                if (i != totalBots - 1) //If it is not the last one
                {
                    ECB.SetComponentEnabled<BackBotTag>(instance, false);
                }
                else
                {
                    ECB.SetComponent(instance, BackColor());
                    ECB.AddComponent<TeamReadyTag>(instance);
                    ECB.SetComponentEnabled<TeamReadyTag>(instance, false);
                }

                //This is not really useful yet
                ECB.SetComponentEnabled<ReachedTarget>(instance, false);
                ECB.SetComponentEnabled<CarryingBotTag>(instance, false);

                ECB.SetComponent<BotTag>(instance, new BotTag
                {
                    cooldown = 0.0f,
                    noInChain = i,
                    indexInChain = 0
                });

                ECB.AddSharedComponent(instance, new Team { Value = t });

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
        var TransitionManager = SystemAPI.GetSingletonEntity<Transition>();
        ECB.AddComponent<botSpawnCompleteTag>(TransitionManager);
       
        
    }
}
