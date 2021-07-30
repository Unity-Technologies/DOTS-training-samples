using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class BotThrowerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecbs = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        var ecb = ecbs.CreateCommandBuffer();

        Entities.ForEach((Entity bot, ref BotThrower thrower, ref BotActionUndefined action) =>
            {
                ecb.RemoveComponent<BotActionUndefined>(bot);
                ecb.AddComponent<BotActionPickBucket>(bot);
            }
        ).Schedule();

        Entities.ForEach((Entity bot, ref BotThrower scooper, ref BotActionPickBucket action) =>
            {
                if (action.ActionDone)
                {
                    ecb.RemoveComponent<BotActionPickBucket>(bot);
                    ecb.AddComponent<BotActionEmptyBucket>(bot);
                }
            }
        ).Schedule();
        
        Entities.ForEach((Entity bot, ref BotThrower scooper, ref BotActionEmptyBucket action) =>
            {
                if (action.ActionDone)
                {
                    ecb.RemoveComponent<BotActionEmptyBucket>(bot);
                    ecb.AddComponent<BotActionDropBucket>(bot);
                }
            }
        ).Schedule();
        
        Entities.ForEach((Entity bot, ref BotThrower scooper, ref BotActionDropBucket action) =>
            {
                if (action.ActionDone)
                {
                    ecb.RemoveComponent<BotActionDropBucket>(bot);
                    ecb.AddComponent<BotActionPickBucket>(bot);
                }
            }
        ).Schedule();
        
        ecbs.AddJobHandleForProducer(Dependency);
    }
}