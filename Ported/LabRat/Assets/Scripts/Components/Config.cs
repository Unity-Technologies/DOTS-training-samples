

using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct Config : IComponentData
{
    public float MouseMovementSpeed;
    public float CatMovementSpeed;

    public float MouseSpawnRate;

    public uint MapSeed;
    public int MapWidth;
    public int MapHeight;

    public int CatsInMap;
    
    public Entity CatPrefab;
    public Entity MousePrefab;
    public Entity ExitPrefab;
    public Entity ArrowPrefab;
}
