using Unity.Entities;
using UnityEngine;

public partial struct WaterSpawningSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }

    public void OnUpdate(ref SystemState state)
    {
        Debug.Log('h');
    }
}
