using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class TornadoMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var time = Time.ElapsedTime;

        Entities.WithAll<Tornado>().ForEach((Entity entity, ref Translation transl) => {
            transl.Value.x = math.cos((float)time / 6f) * 30f;
            transl.Value.z = math.sin((float)time / 6f*1.618f) * 30f;
            //tornadoX = Mathf.Cos(Time.time / 6f) * 30f;
            //tornadoZ = Mathf.Sin(Time.time / 6f * 1.618f) * 30f;
            //cam.position = new Vector3(tornadoX, 10f, tornadoZ) - cam.forward * 60f;
        }).Run();
    }
}
