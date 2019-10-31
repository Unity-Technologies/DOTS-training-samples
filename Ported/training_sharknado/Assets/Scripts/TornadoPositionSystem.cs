using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class TornadoPositionSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref TornadoPosition tornadoPos) =>
        {
            tornadoPos.position = new float3(Mathf.Cos(Time.time / 6f) * 30f, 0,
           Mathf.Sin(Time.time / 6f * 1.618f) * 30f);
        });
    }
}
