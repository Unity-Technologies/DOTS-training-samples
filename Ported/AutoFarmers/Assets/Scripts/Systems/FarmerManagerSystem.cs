using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct FarmerManagerSystem: ISystem
{
    public void OnCreate(ref SystemState state)
    {
        
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer command = new EntityCommandBuffer(Allocator.Temp);

        foreach (var creator in SystemAPI.Query<RefRO<Ecsprefabcreator>>())
        {
            Entity ent= command.Instantiate(creator.ValueRO.prefab);
            command.SetComponent(ent,new FarmerSpeed
            {
                MovementSpeed = 5f
            });
        }
        command.Playback(state.EntityManager);
        state.Enabled = false;
    }
}