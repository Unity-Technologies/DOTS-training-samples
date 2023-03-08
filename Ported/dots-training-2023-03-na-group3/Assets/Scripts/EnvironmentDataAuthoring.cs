using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// encapsulates data needed to build the environment
/// Use this as a readonly singleton to global values like bounds of the environment, ant's speed, etc... 
/// </summary>
public class EnvironmentDataAuthoring : MonoBehaviour
{
    [Tooltip("Ref point as the center of the environment for bounds calculations." +
             " This doesn't have to be the Ant's spawn point.")]
    public float3 Center;
    [Tooltip("extents of the environment on x,z plane (like width and height of a rectangle)")]
    public float2 Extents; 
    [Tooltip("number of cells on x, z plane. The whole environment will be divided into resolution*resolution cells. each cell keeps a pheromone value")]
    public int resolution;
    [Tooltip("The rate in which ants generate pheromones")]
    public float DropRate;
    [Tooltip("The rate in which pheromones in the environment evaporate")]
    public float FadeRate;
}

public struct EnvironmentData : IComponentData
{
    public float3 Center;
    public float2 Extents; //extents of the environment on x,z plane 
    public int resolution; //number of cells on x, z plane
    public float DropRate;
    public float FadeRate;
}

public class EnvironmentDataBaker : Baker<EnvironmentDataAuthoring>
{
    public override void Bake(EnvironmentDataAuthoring authoring)
    {
        AddComponent(new EnvironmentData
        {
                Center = authoring.Center,
                Extents = authoring.Extents,
                resolution = authoring.resolution,
                DropRate = authoring.DropRate,
                FadeRate = authoring.FadeRate
        });
    }
}