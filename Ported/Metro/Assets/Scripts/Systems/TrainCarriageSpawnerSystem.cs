using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public partial struct TrainCarriageSpawnerSystem : ISystem
{
    public bool hasRun;
    
    public void OnCreate(ref SystemState state)
    {
        hasRun = false;
    }

    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (RefRO<Train> train in SystemAPI.Query<RefRO<Train>>())
        {
            //Get child entities and assign the references
            if(!SystemAPI.HasBuffer<Child>(train.ValueRO.entity))
            {
                continue;
            }

            hasRun = true;
            
            DynamicBuffer<Child> childFromEntity = SystemAPI.GetBuffer<Child>(train.ValueRO.entity);
            foreach (Child child in childFromEntity)
            {
                if (!SystemAPI.HasComponent<Carriage>(child.Value))
                {
                    continue;
                }
                
                Carriage c = SystemAPI.GetComponent<Carriage>(child.Value);
                c.ownerTrainID = train.ValueRO.entity;
                SystemAPI.SetComponent(child.Value, c);
            }
        }

        state.Enabled = !hasRun;
    }
}
