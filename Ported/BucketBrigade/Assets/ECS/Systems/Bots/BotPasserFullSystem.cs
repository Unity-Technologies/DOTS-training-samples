using Unity.Entities;
using UnityEngine;

public class BotPasserFullSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecbs = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        var ecb = ecbs.CreateCommandBuffer();

        Entities.ForEach((Entity bot, ref BotPasserFull passer, ref BotActionUndefined action) =>
            {
                ecb.RemoveComponent<BotActionUndefined>(bot);
                ecb.AddComponent<BotActionPickBucket>(bot);
                Debug.Log("PasserFull => BotActionPickBucket");
            }
        ).Schedule();

        Entities.ForEach((Entity bot, ref BotPasserFull scooper, ref BotActionPickBucket action) =>
            {
                if (action.ActionDone)
                {
                    ecb.RemoveComponent<BotActionPickBucket>(bot);
                    ecb.AddComponent<BotActionDropBucket>(bot);
                    Debug.Log("PasserFull => BotActionDropBucket");
                }
            }
        ).Schedule();
        
        Entities.ForEach((Entity bot, ref BotPasserFull scooper, ref BotActionDropBucket action) =>
            {
                if (action.ActionDone)
                {
                    ecb.RemoveComponent<BotActionDropBucket>(bot);
                    ecb.AddComponent<BotActionPickBucket>(bot);
                    Debug.Log("PasserFull => BotActionPickBucket");
                }
            }
        ).Schedule();
        
        ecbs.AddJobHandleForProducer(Dependency);
    }
}