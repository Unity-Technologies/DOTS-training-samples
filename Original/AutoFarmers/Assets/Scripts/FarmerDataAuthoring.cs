using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;


public struct FarmerData_Spawner:IComponentData
{
    public Entity prefab;
}

public struct DroneData_Spawner:IComponentData
{
    public Entity prefab;
}

public struct WorkerTeleport:IComponentData {
    public float2 position;
}

public struct WorkerDataCommon:IComponentData
{
    public Entity plantAttached;
    public Entity storeToSellTo;
}

public struct Farmer:IComponentData
{
}

public struct Drone:IComponentData
{
}