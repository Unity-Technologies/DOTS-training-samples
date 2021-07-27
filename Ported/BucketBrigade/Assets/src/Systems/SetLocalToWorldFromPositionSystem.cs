using System;
using src.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace src.Systems
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class SetLocalToWorldFromPositionSystem : SystemBase
    {

        protected override void OnUpdate()
        {
            Entities.WithBurst().ForEach((ref LocalToWorld localToWorld, in Position pos) =>
            {
                //localToWorld.Value.c3 = new float4(pos.Value.x, 0, pos.Value.y, 0);
                localToWorld.Value = float4x4.TRS(new float3(pos.Value.x, 0, pos.Value.y), Quaternion.identity, Vector3.one);
            }).ScheduleParallel();
        }
    }
}
