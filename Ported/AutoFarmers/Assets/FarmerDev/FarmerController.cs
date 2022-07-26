using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct FarmerController : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        state.GetComponentDataFromEntity<Farmer>();
    }
}
