using System.ComponentModel.Design.Serialization;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;


[BurstCompile]
partial struct SpeedModifiyerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }
 
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
 
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        

      

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            foreach (RefRW<Ant> ant in SystemAPI.Query<RefRW<Ant>>())
                ant.ValueRW.Speed = config.AntSpeeds[0];
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            foreach (RefRW<Ant> ant in SystemAPI.Query<RefRW<Ant>>())
                ant.ValueRW.Speed = config.AntSpeeds[1];
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            foreach (RefRW<Ant> ant in SystemAPI.Query<RefRW<Ant>>())
                ant.ValueRW.Speed = config.AntSpeeds[2];
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            foreach (RefRW<Ant> ant in SystemAPI.Query<RefRW<Ant>>())
                ant.ValueRW.Speed = config.AntSpeeds[3];
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            foreach (RefRW<Ant> ant in SystemAPI.Query<RefRW<Ant>>())
                ant.ValueRW.Speed = config.AntSpeeds[4];
        }

    }
}