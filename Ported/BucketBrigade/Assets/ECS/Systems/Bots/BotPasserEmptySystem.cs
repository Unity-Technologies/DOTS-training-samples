using Unity.Entities;
using UnityEngine;

public class BotPasserEmptySystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecbs = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        var ecb = ecbs.CreateCommandBuffer();

        Entities.ForEach((Entity bot, ref BotPasserEmpty passer, ref BotActionUndefined action) =>
            {
                ecb.RemoveComponent<BotActionUndefined>(bot);
                ecb.AddComponent<BotActionPickEmptyBucket>(bot);
                Debug.Log("PasserEmpty => BotActionPickBucket");
            }
        ).Schedule();

        Entities.ForEach((Entity bot, ref BotPasserEmpty passer, ref BotActionPickEmptyBucket action) =>
            {
                if (action.ActionDone)
                {
                    ecb.RemoveComponent<BotActionPickEmptyBucket>(bot);
                    ecb.AddComponent<BotActionDropBucket>(bot);
                    Debug.Log("PasserEmpty => BotActionDropBucket");
                }
            }
        ).Schedule();
        
        Entities.ForEach((Entity bot, ref BotPasserEmpty passer, ref BotActionDropBucket action) =>
            {
                if (action.ActionDone)
                {
                    ecb.RemoveComponent<BotActionDropBucket>(bot);
                    ecb.AddComponent<BotActionPickEmptyBucket>(bot);
                    Debug.Log("PasserEmpty => BotActionPickEmptyBucket");
                }
            }
        ).Schedule();
        
        ecbs.AddJobHandleForProducer(Dependency);
    }
}