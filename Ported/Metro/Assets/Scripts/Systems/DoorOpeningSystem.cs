using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class DoorOpeningSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;    

        Entities.ForEach((  Entity entity,
                            ref Translation translation,
                            ref DoorController doorController) =>
        {
            Entity leader = Entity.Null;

            if (HasComponent<Ticker>(doorController.Cart))
                leader = doorController.Cart;
            else
            {
                leader = GetComponent<Follower>(doorController.Cart).Leader;
            }

            var ticker = GetComponent<Ticker>(leader);
            
            if (ticker.TimeRemaining >= 0)
            {
                if (ticker.TimeRemaining >= 4)
                    translation.Value = new float3(math.smoothstep(0,1,5 - ticker.TimeRemaining), 0, 0);
                else if (ticker.TimeRemaining <= 1)
                    translation.Value = new float3(math.smoothstep(0,1,ticker.TimeRemaining), 0, 0);
            }
        }).ScheduleParallel();
    }
}
