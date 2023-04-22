using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

namespace Systems
{
    [UpdateAfter(typeof(BotSpawningSystem))]
    [UpdateAfter(typeof(GridTilesSpawningSystem))]
    [UpdateAfter(typeof(WaterSpawningSystem))]
    [UpdateAfter(typeof(BucketSpawningSystem))]
    public partial struct InitializeChainIndecies : ISystem
    {
        private int numTimesRun;
        private int i;
        private int j;

        private EntityQuery BotTagsFQ;
        private EntityQuery BotTagsBQ;
        
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<botSpawnCompleteTag>();
            state.RequireForUpdate<Config>();
        }
        
       

        public void OnUpdate(ref SystemState state)
        {
            
            var config = SystemAPI.GetSingleton<Config>();
            var numTeams = config.TotalTeams;
            
            //Get delta time
            var dt = SystemAPI.Time.DeltaTime;
        
            //GIGANTIC FOR LOOP AGAIN 
            //Get all teams
            var teamList = new NativeList<Team>(numTeams, Allocator.Persistent);
      
            //Get component for each team
            for (int t = 0; t < numTeams; t++)
            {
                var TeamComponent = new Team { Value = t};
      
                teamList.Add(TeamComponent);
            }


            //Add gigantic for loop to handle each team 
            for (int i = 0; i < teamList.Length; i++)
            {
                BotTagsFQ = SystemAPI.QueryBuilder().WithAll<BotTag, ForwardPassingBotTag,Team>()
                    .Build();
                BotTagsFQ.SetSharedComponentFilter(teamList[i]);
                BotTagsBQ = SystemAPI.QueryBuilder().WithAll<BotTag, BackwardPassingBotTag,Team>()
                    .Build();
                BotTagsBQ.SetSharedComponentFilter(teamList[i]);
                
                //Create native arrays
                var botTagsF = BotTagsFQ.ToComponentDataArray<BotTag>(Allocator.Temp);
                var botTagsB = BotTagsBQ.ToComponentDataArray<BotTag>(Allocator.Temp);

                var botEntityF = BotTagsFQ.ToEntityArray(Allocator.Temp);
                var botEntityB = BotTagsBQ.ToEntityArray(Allocator.Temp);


                for (int k = 0; k < botTagsF.Length; k++)
                {
                    var newBotTagF = botTagsF[k];
                    newBotTagF.indexInChain = k;
                    state.EntityManager.SetComponentData(botEntityF[k],newBotTagF);
                }
            
                for (j = botTagsB.Length-1; j >= 0 ; j--)
                {
                    var newBotTagB = botTagsB[j];
                    newBotTagB.indexInChain = j;
                    state.EntityManager.SetComponentData(botEntityB[j],newBotTagB);
                }
                
                
                botEntityF.Dispose();
                botTagsF.Dispose();
                botEntityB.Dispose();
                botTagsB.Dispose();


            }
            
            Debug.Log("Ran InitializeChainIndex System" + " i: " + i + " j: " + j);
            
            state.Enabled = false;
            
            





        }
    }
}
