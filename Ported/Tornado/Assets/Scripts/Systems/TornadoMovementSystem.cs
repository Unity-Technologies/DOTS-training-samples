using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

public class TornadoMovementSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<GameObjRefs>();
    }

    protected override void OnUpdate()
    {
        var camera = this.GetSingleton<GameObjRefs>().Camera;

        var time = Time.ElapsedTime;

        float3 camTransl = 0f;

        Entities.WithAll<Tornado>().ForEach((Entity entity, ref Translation transl) => {
            transl.Value.x = math.cos((float)time / 6f) * 30f;
            transl.Value.z = math.sin((float)time / 6f*1.618f) * 30f;

            camTransl = transl.Value;
        }).Run();

        camera.transform.position = new Vector3(camTransl.x, 10f, camTransl.z) - camera.transform.forward * 60f;
    }
}
