using Metro;
using Unity.Entities;
using UnityEngine;


public partial struct PassengerSystem : ISystem
{
    public void OnCreate(ref SystemState state) { }

    public void OnDestroy(ref SystemState state) { }

    public void OnUpdate(ref SystemState state)
    {
        ///Debug.Log($"Update Passenger {state.World.Name}");
        var singleton = SystemAPI.GetSingleton<ConfigAuthoring.Config>();
        Debug.Log($"NumPassengers: {singleton.NumPassengers}");
    }
}
