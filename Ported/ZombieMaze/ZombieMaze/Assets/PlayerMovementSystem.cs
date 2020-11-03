using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public struct Position : IComponentData
{
    public float2 Value;
}

public struct Direction : IComponentData
{
    public float2 Value;
}

public class PlayerMovementSystem : SystemBase
{
    protected override void OnCreate()
    {

    }

    protected override void OnUpdate()
    {
   
    }
}
