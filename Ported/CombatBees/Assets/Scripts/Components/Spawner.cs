using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct Spawner : IComponentData
{
    public Entity YellowBeePrefab;
    public Entity BlueBeePrefab;
    public Entity BloodPrefab;
    public Entity BeeBitsPrefab;
    public Entity ResourcePrefab;

    [Min(0)]
    public int StartingBees;
    
    [Min(0)]
    public int StartingResources;

    public int GoalDepth
    {
        get { return 10; }
    }
    
    public int2 ArenaExtents
    {
        get { return new int2(40, 15); }
    }
    
    public int ArenaHeight
    {
        get { return 20; }
    }
}