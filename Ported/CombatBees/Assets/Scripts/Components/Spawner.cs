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

    [Range(0,1.0f)]
    public float ChanceToAttack;

    public static int GoalDepth
    {
        get { return 10; }
    }

    public static int2 ArenaExtents
    {
        get { return new int2(40, 15); }
    }

    public static int ArenaHeight
    {
        get { return 20; }
    }
}