using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Serialization;

[Serializable]
[GenerateAuthoringComponent]
public struct GameSceneData : IComponentData
{
    public int FloorSizeX;
    public int FloorSizeZ;
    public int TeamFloorSizeX;
    public float TeamZoneHeight;

    public int StartResourcesCount;
    public int StartBeesCountPerTeam;

    public Box ResourcesSpawnBox;
}
