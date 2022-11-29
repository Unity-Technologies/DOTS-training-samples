using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

struct Config : IComponentData
{
    //const config data
    public Entity WallPrefab;
    public Entity AntPrefab;

    public int TotalAmountOfAnts;
    public int PlaySize;    // Size of play area edge, area center at origin.
    public int AmountOfWalls;
    public float TimeScale; // Simulation time scale, multiply with SystemAPI.Time.DeltaTime
}
