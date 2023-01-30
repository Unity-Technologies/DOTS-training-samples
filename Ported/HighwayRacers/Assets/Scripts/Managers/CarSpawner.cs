using Unity.Entities;
using UnityEngine;

public struct CarSpawner : IComponentData
{
    public Entity carPrefab;    
    
    public int amount;

    public bool spawned;
    
    public int NumLanes;
    
    public float LengthLanes;

    public float MinVelocity;
    public float MaxVelocity;

}
