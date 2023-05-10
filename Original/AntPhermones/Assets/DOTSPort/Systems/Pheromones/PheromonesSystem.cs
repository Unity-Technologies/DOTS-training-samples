using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public partial struct PheromonesSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<AntData>();
        state.RequireForUpdate<PheromoneBufferElement>();
    }

    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false; // temporary, until implementation
    }

    public void OnDestroy(ref SystemState state)
    {
    }
}



