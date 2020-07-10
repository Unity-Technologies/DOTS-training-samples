using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct GameSettings : IComponentData
{
    [Header("PathFinding")]
    public bool PathfindingEnabled; // Could become an enum if we want to switch between pathfinding solutions

    // Finding fields to till
    [Header("Tilling")]
    public int2 MinFieldSize;
    public int2 MaxFieldSize;
    [Tooltip("Number of tries before a farmer gives up finding a farm in a single frame")]
    public int NumFindFieldTriesPerFrame; // = 10;     // if we don't find anything suitable within 10 tries, let it go for now
    [Tooltip("Half of the tries, try to find the field inside this radius around the farmer's location")]
    public int LocalFieldSearchRadius; // = 10; // this "square" radius should allow allow for 16 fullsized fields, enough to start looking?
}
