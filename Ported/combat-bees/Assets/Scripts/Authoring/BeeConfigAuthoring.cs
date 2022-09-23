using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor.Build.CacheServer;
using UnityEngine;
using UnityEngine.Rendering;

class BeeConfigAuthoring : UnityEngine.MonoBehaviour
{
    public GameObject BeePrefab;
    public GameObject FoodPrefab;
    public int BeeCount;
    public int FoodCount;
    public GameObject Field;
    [Range(0,1)]
    public float Aggressivity;
    public float3 InitVel;
    public float BeeSpeed = 50;
    public float Gravity = -9.81f;
    public float ObjectSize = 0.1f;
}

[UpdateBefore(typeof(NestAuthoring))]
class BeeConfigBaker : Baker<BeeConfigAuthoring>
{
    public override void Bake(BeeConfigAuthoring authoring)
    {
        var renderer = authoring.Field.GetComponent<Renderer>();
        AddComponent<BeeConfig>(new BeeConfig()
        {
            bee = GetEntity(authoring.BeePrefab),
            food = GetEntity(authoring.FoodPrefab),
            beeCount = authoring.BeeCount,
            foodCount = authoring.FoodCount,
            aggressivity = authoring.Aggressivity,
            fieldArea = AABBExtensions.ToAABB(renderer.bounds),
            initVel = authoring.InitVel,
            beeSpeed = authoring.BeeSpeed,
            gravity = authoring.Gravity,
            objectSize = authoring.ObjectSize
        });
    }
}
