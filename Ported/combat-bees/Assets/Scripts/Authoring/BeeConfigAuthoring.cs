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
    public float3 InitVel;
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
            fieldArea = AABBExtensions.ToAABB(renderer.bounds),
            initVel = authoring.InitVel
        });
    }
}
