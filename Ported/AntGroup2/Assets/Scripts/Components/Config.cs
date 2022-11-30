using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

struct Config : IComponentData
{
    //const config data
    public Entity WallPrefab;
    public Entity AntPrefab;
    public Entity FoodPrefab;
    public Entity ColonyPrefab;

    public int TotalAmountOfAnts;
    public int PlaySize;    // Size of play area edge, area center at origin.
    public int AmountOfWalls;
    public float TimeScale; // Simulation time scale, multiply with SystemAPI.Time.DeltaTime
    public float RandomSteeringAmount;

    public int PheromoneSampleDistPixels;   // Neighborhood size, -pixels < x < pixels
    public int PheromoneSampleStepAngle;
    public int PheromoneSampleStepCount;
    public int PheromoneSpawnDistPixels;
    public float PheromoneSpawnRateSec;      // + amount per iteration
    public float PheromoneDecayRateSec;     // - amount per iteration per dT
}
