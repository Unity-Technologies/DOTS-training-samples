using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

namespace Systems
{
    [UpdateAfter(typeof(BotSpawningSystem))]
    [UpdateAfter(typeof(BotNearestMovementSystem))]
    [UpdateBefore(typeof(BotMovementSystem))]
    public partial struct InitializeChainIndecies : ISystem
    {
        private int numTimesRun;
        private int i;
        private int j;

        private EntityQuery BotTagsFQ;
        private EntityQuery BotTagsBQ;
        
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<spawnCompleteTag>();
            BotTagsFQ = new EntityQueryBuilder(Allocator.Persistent).WithAll<BotTag, ForwardPassingBotTag>()
                .Build(state.EntityManager);
            BotTagsBQ = new EntityQueryBuilder(Allocator.Persistent).WithAll<BotTag, BackwardPassingBotTag>()
                .Build(state.EntityManager);
        }
        
       

        public void OnUpdate(ref SystemState state)
        {
            //Create native arrays
            var botTagsF = BotTagsFQ.ToComponentDataArray<BotTag>(Allocator.Temp);
            var botTagsB = BotTagsBQ.ToComponentDataArray<BotTag>(Allocator.Temp);

            var botEntityF = BotTagsFQ.ToEntityArray(Allocator.Temp);
            var botEntityB = BotTagsBQ.ToEntityArray(Allocator.Temp);


            for (i = 0; i < botTagsF.Length; i++)
            {
                var newBotTagF = botTagsF[i];
                newBotTagF.indexInChain = i;
                state.EntityManager.SetComponentData(botEntityF[i],newBotTagF);
            }
            
            for (j = 0; j < botTagsB.Length; j++)
            {
                var newBotTagB = botTagsB[j];
                newBotTagB.indexInChain = j;
                state.EntityManager.SetComponentData(botEntityB[j],newBotTagB);
            }
            
            
            
            /*
            foreach (var (botTagF,entity) in SystemAPI.Query<BotTag>().WithAll<ForwardPassingBotTag>().WithEntityAccess())
            {
                var newBotTagF = botTagF;
                newBotTagF.indexInChain = i;
                state.EntityManager.SetComponentData(entity,newBotTagF);
                i++;
            }
        
            foreach ( var (botTagB,entity) in SystemAPI.Query<BotTag>().WithAll<BackwardPassingBotTag>().WithEntityAccess())
            {
                var newBotTagB = botTagB;
                newBotTagB.indexInChain = j;
                state.EntityManager.SetComponentData(entity,newBotTagB);
                j++;
            }
           
            numTimesRun++;

            */
            Debug.Log("Ran InitializeChainIndex System" + " i: " + i + " j: " + j);
            
            state.Enabled = false;
            botEntityF.Dispose();
            botTagsF.Dispose();
            botEntityB.Dispose();
            botTagsB.Dispose();
            





        }
    }
}
