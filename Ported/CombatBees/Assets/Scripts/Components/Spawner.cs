using Unity.Entities;
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
}