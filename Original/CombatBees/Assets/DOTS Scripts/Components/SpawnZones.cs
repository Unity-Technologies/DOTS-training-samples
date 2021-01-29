﻿using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct SpawnZones : IComponentData
{
    public AABB LevelBounds;
    
    public AABB Team1Zone;
    public AABB Team2Zone;
    
    public Entity BeeTeam1Prefab;
    public Entity BeeTeam2Prefab;
    public Entity FoodPrefab;
    public Entity BloodPrefab;

    public int BeesPerFood;
    public float FlightJitter;
}
