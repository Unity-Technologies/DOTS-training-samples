using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
public partial struct PlayerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();

        float dt = SystemAPI.Time.DeltaTime;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        float playerInfluenceDist = 10;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (var transformPlayer in SystemAPI.Query<TransformAspect>().WithAll<Player>())
            {
                foreach (var (speed, transform) in SystemAPI.Query<RefRW<Speed>, TransformAspect>().WithAll<Ball>())
                {
                    var diff = transform.LocalPosition - transformPlayer.LocalPosition;

                    if(math.lengthsq(diff) < (playerInfluenceDist * playerInfluenceDist))
                    {
                        speed.ValueRW.speed = math.normalize(diff) * config.ImpactVelocity;
                    }
                }
            }
        }

        foreach (var speed in SystemAPI.Query<RefRW<Speed>>().WithAll<Player>())
        {
            speed.ValueRW.speed = new float3(h, 0, v);
        }
    }
}
