using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
[GenerateAuthoringComponent]
public struct GameGlobalData : IComponentData
{
    public Entity BeePrefab;
    public Entity ResourcePrefab;
    public Entity GenericCubePrefab;
    
    public Entity ParticleSparkPrefab;
    public Entity ParticleSmokePrefab;
    public Entity ParticleBloodInFlightPrefab;
    public Entity ParticleBloodSettledPrefab;
    
    public UnityEngine.Color FloorColor;
    public UnityEngine.Color TeamAColor;
    public UnityEngine.Color TeamBColor;
    
    public float Gravity;
    public float GridCellSize;
    public float ResourceHeight;
    public float ResourceSnapTime;
    public float ResourceCarryStiffness;
    public float ResourceCarryOffset;
    public int BeeSpawnCountOnScore;
    
    [Header("Particles")]
    public int Particles_SparksCount;
    public float Particles_SparksVelocity;
    public int Particles_SmokesCount;
    public float Particles_SmokesVelocity;
    public int Particles_BloodsCount;
    public float Particles_BloodsVelocity;
}
