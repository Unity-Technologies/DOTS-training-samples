using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public partial struct ObstacleInitHashSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<CollisionHashSet>();
        state.RequireForUpdate<ObstacleArcPrimitive>();
    }

    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;

       // var hashSet = SystemAPI.GetSingleton<CollisionHashSet>();
       // var collidersBuffer = SystemAPI.GetSingletonBuffer<ObstacleArcPrimitive>();
        

    }
}


public struct CollisionHashSet: IComponentData
{
    public NativeHashSet<bool> CollisionSet;
}
