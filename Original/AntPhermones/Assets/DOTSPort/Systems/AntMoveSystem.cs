//using System.Collections;
//using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
//using UnityEngine;

public partial struct AntMoveSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<AntData>();
    }
    public void OnDestroy(ref SystemState state)
    {

    }
    public void OnUpdate(ref SystemState state)
    {
        foreach (var ant in SystemAPI.Query<RefRW<AntData>, RefRW<LocalTransform>>())
        {

            // TODO: need to scale simulation space to render space
            ant.Item2.ValueRW.Position.x = ant.Item1.ValueRO.Position.x;
            ant.Item2.ValueRW.Position.z = ant.Item1.ValueRO.Position.y;
            ant.Item2.ValueRW.Rotation = quaternion.AxisAngle(new float3(0, 1f, 0), ant.Item1.ValueRO.FacingAngle);
        }
    }
}
