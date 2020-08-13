using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class DebugBots : SystemBase
{
    protected override void OnUpdate()
    {
        // Debug Only
        Entities
            .WithName("DebugNextBots")
            .WithoutBurst()
            .ForEach((Entity e, ref Translation translation, in NextBot nextBot) =>
                {
                    if (nextBot.Value != Entity.Null)
                    {
                        Translation t = EntityManager.GetComponentData<Translation>(nextBot.Value);
                        var start = translation.Value;
                        var end = translation.Value + (t.Value - translation.Value) * 0.8f;
                        Debug.DrawLine(start, end, Color.yellow, 0.1f);
                    }
                    else
                    {
                        var start = translation.Value;
                        var end = translation.Value + new float3(0, 2, 0); 
                        Debug.DrawLine(start, end, Color.red, 0.1f);
                    }
                }
            ).Run();
    }
}
