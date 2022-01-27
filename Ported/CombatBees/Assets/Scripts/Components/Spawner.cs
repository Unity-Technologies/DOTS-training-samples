using Unity.Entities;
using Unity.Mathematics;
using Unity.Properties.UI;
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

    [MinMax(0,1.0f)]
    public float ChanceToAttack;

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