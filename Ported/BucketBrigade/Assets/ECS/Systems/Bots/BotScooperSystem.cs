using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class BotScooperSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecbs = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        var ecb = ecbs.CreateCommandBuffer();

        Entities.ForEach((Entity bot, ref BotScooper scooper, ref BotActionUndefined action) =>
            {
                ecb.RemoveComponent<BotActionUndefined>(bot);
                ecb.AddComponent<BotActionFindBucket>(bot);
            }
        ).Schedule();

        Entities.ForEach((Entity bot, ref BotScooper scooper, ref BotActionFindBucket action) =>
            {
                if (action.ActionDone)
                {
                    ecb.RemoveComponent<BotActionFindBucket>(bot);
                    ecb.AddComponent<BotActionFillBucket>(bot);
                }
            }
        ).Schedule();
        
        Entities.ForEach((Entity bot, ref BotScooper scooper, ref BotActionFillBucket action) =>
            {
                if (action.ActionDone)
                {
                    ecb.RemoveComponent<BotActionFillBucket>(bot);
                    ecb.AddComponent<BotActionDropBucket>(bot);
                }
            }
        ).Schedule();
        
        Entities.ForEach((Entity bot, ref BotScooper scooper, ref BotActionDropBucket action) =>
            {
                if (action.ActionDone)
                {
                    ecb.RemoveComponent<BotActionDropBucket>(bot);
                    ecb.AddComponent<BotActionFindBucket>(bot);
                }
            }
        ).Schedule();
        
        ecbs.AddJobHandleForProducer(Dependency);
    }
}