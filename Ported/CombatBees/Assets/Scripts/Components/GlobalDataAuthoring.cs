using System;
using System.Collections.Generic;
using System.Numerics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityGameObject = UnityEngine.GameObject;
using UnityTransform = UnityEngine.Transform;
using UnityRangeAttribute = UnityEngine.RangeAttribute;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

public class GlobalDataAuthoring : UnityMonoBehaviour
    , IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    [Serializable]
    public class LocalTeamDef
    {
        public float Speed;
        public float AttackRange;
        public float PickupFoodRange;
        public float HuntTimeout;
        [Range(0.0f, 1.0f)]
        public float FlutterMagnitude;
        [Range(0.1f, 2.0f)]
        public float FlutterInterval;
        [Range(0.0f,1.0f)]
        public float Aggression;
    }
    
    public UnityGameObject BeePrefab;
    public UnityGameObject FoodPrefab;
    public UnityGameObject GibletPrefab;
    public UnityGameObject ExplosionPrefab;
    public int StartingFoodCount;
    public int BeeCount;
    public int BeeExplosionCount;
    public float Length;
    public float Width;
    public float HiveDepth;
    public float MinimumSpeed;
    public float TurnbackWidth;
    public float3 FlutterMagnitude;
    public float3 FlutterInterval;
    public float DecayTime;
    public float TimeBetweenIdleUpdates;
    public LocalTeamDef[] TeamDefinitions;

    public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(BeePrefab);
        referencedPrefabs.Add(FoodPrefab);
        referencedPrefabs.Add(GibletPrefab);
        referencedPrefabs.Add(ExplosionPrefab);
    }

    // This function is required by IConvertGameObjectToEntity
    public void Convert(Entity entity, EntityManager dstManager
        , GameObjectConversionSystem conversionSystem)
    {
        float hd = HiveDepth / 2.0f;
        float3 max = new float3(Length / 2.0f, Width / 2.0f, Width / 2.0f);
        float3 min = new float3(Length / -2.0f, Width / -2.0f, Width / -2.0f);
        float3 minHive = new float3(min.x + hd, 0.0f, 0.0f);
        float3 maxHive = new float3(max.x - hd, 0.0f, 0.0f);
        
        dstManager.AddComponentData(entity, new GlobalData
        {
            BeePrefab = conversionSystem.GetPrimaryEntity(BeePrefab),
            FoodPrefab = conversionSystem.GetPrimaryEntity(FoodPrefab),
            GibletPrefab = conversionSystem.GetPrimaryEntity(GibletPrefab),
            ExplosionPrefab = conversionSystem.GetPrimaryEntity(ExplosionPrefab),
            BoundsMax = max,
            BoundsMin = min,
            BlueHiveCenter = minHive,
            YellowHiveCenter = maxHive,
            HiveDepth = HiveDepth,
            StartingFoodCount = StartingFoodCount,
            BeeCount = BeeCount,
            BeeExplosionCount = BeeExplosionCount,
            MinimumSpeed = MinimumSpeed,
            TurnbackZone = max - new float3(TurnbackWidth),
            TurnbackWidth = TurnbackWidth,
            DecayTime = math.max(DecayTime,0.01f),
            TimeBetweenIdleUpdates = TimeBetweenIdleUpdates
        });

        var buffer = dstManager.AddBuffer<TeamDefinition>(entity);
        foreach (var team in TeamDefinitions)
        {
            buffer.Add(new TeamDefinition()
            {
                aggression = team.Aggression,
                speed = team.Speed,
                attackRange = team.AttackRange,
                huntTimeout = team.HuntTimeout,
                pickupFoodRange = team.PickupFoodRange,
                flutterMagnitude = team.FlutterMagnitude * FlutterMagnitude,
                flutterInterval = team.FlutterInterval * FlutterInterval
            });
        }
    }
}