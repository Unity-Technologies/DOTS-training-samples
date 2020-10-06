using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class ScooperSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float elapsedTime = (float)Time.ElapsedTime;
        
        Entities.ForEach((Entity e, in AgentTags.ScooperTag scooper) =>
        {
            // do scooper stuff.
            
            //SetComponent<URPMaterialPropertySpecColor>(e, new URPMaterialPropertySpecColor{ Value = new float4(1.0f, math.clamp((float)math.sin(elapsedTime), 0.0f, 1.0f), 0.0f, 1.0f) } ); 
            var t = GetComponent<Translation>(e);
            t.Value.y = math.clamp((float) math.sin(elapsedTime), 0.0f, 1.0f) + 1.0f;
            //t.Value = new float3(t.Value.x, 1.0f, t.Value.z ) + new float3(0.0f, 0.0f, 0.0f);
            SetComponent<Translation>(e, t);


        }).Run();
    }
}
