using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct AnimVolumeSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Water>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var newElapsedTime = (float)SystemAPI.Time.ElapsedTime;
        Debug.Log(newElapsedTime);
        new AnimVolumeJob
        {
            ElapsedTime = newElapsedTime
        }.ScheduleParallel();
    }
}

[WithAll(typeof(Water))]
[BurstCompile]
partial struct AnimVolumeJob : IJobEntity
{
    public float ElapsedTime;
    
    void Execute(ref LocalTransform transform)
    {
        Debug.Log(math.sin(ElapsedTime));
        transform = transform.ApplyScale(math.sin(ElapsedTime));
    }
}

