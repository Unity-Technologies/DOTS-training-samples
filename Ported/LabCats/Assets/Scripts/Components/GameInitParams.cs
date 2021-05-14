using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public struct GameInitParams : IComponentData
{
    public int LengthGame;
    public uint BoardGenerationSeed;
    public uint AIControllerSeed;
    public float WallDensity;
    public int MaximumNumberCellsPerHole;
}
