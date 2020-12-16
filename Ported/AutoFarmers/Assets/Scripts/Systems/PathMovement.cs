using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class PathMovement : SystemBase
{
    protected override void OnUpdate()
    {
        var pathBuffers = this.GetBufferFromEntity<PathNode>();
        
        Entities
            .ForEach((Entity entity, ref Path path, ref Velocity velocity, in Translation translation) =>
            {
                // Evaluate the path.
                // var pathNodes = pathBuffers[entity];
                // var targetPosition = pathNodes[path.Index].Value;
                //
                // var farmerPosition = new int2((int)math.floor(translation.Value.x), 
                //                           (int)math.floor(translation.Value.z));
                //
                // var v = math.normalize(farmerPosition - targetPosition);
                // velocity.Value = new float3(v.x, 0f, v.y);
                //
                // // Debug draw path.
                // for (int i = 0; i < pathNodes.Length - 1; i++) {
                //     Debug.DrawLine(new Vector3(pathNodes[i].Value.x + .5f,.5f,pathNodes[i].Value.y + .5f), 
                //         new Vector3(pathNodes[i + 1].Value.x + .5f,.5f,pathNodes[i + 1].Value.y + .5f),
                //         Color.red);
                // }

            }).Run();
    }
}