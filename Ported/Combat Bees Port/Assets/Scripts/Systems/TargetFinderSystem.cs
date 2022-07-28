using System;
using System.Security.Cryptography;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Random = Unity.Mathematics.Random;



[BurstCompile]
partial struct TargetFinderSystem : ISystem
{
    private EntityQuery _foodQuery;
    private EntityQuery _yellowQuery;
    private EntityQuery _blueQuery;
    
    ComponentDataFromEntity<Bee> _beeComponent;
    Random rnd;


    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        var allocator = state.WorldUnmanaged.UpdateAllocator.ToAllocator;

        var yellowQueryBuilder = new EntityQueryDescBuilder(Allocator.Temp);
        yellowQueryBuilder.AddAll(ComponentType.ReadWrite<YellowTeam>());
        yellowQueryBuilder.FinalizeQuery();
        _yellowQuery = state.GetEntityQuery(yellowQueryBuilder);

        var blueQueryBuilder = new EntityQueryDescBuilder(Allocator.Temp);
        blueQueryBuilder.AddAll(ComponentType.ReadWrite<BlueTeam>());
        blueQueryBuilder.FinalizeQuery();
        _blueQuery = state.GetEntityQuery(blueQueryBuilder);

        var foodQueryBuilder = new EntityQueryDescBuilder(Allocator.Temp);
        foodQueryBuilder.AddAll(ComponentType.ReadWrite<NotCollected>());
        foodQueryBuilder.FinalizeQuery();
        _foodQuery = state.GetEntityQuery(foodQueryBuilder);

        _beeComponent = state.GetComponentDataFromEntity<Bee>(false);
    }

    public void OnDestroy(ref SystemState state)
    {
        
    }


    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        
        _beeComponent.Update(ref state);

        rnd = Random.CreateFromIndex(state.GlobalSystemVersion);
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        bool aggressive;
        var allocator = state.WorldUnmanaged.UpdateAllocator.ToAllocator;

        NativeArray<Entity> yellowBees = _yellowQuery.ToEntityArray(allocator);
        NativeArray<Entity> blueBees = _blueQuery.ToEntityArray(allocator);
        NativeArray<Entity> food = _foodQuery.ToEntityArray(allocator);
        

        TargetFinderSystem tempTFS = this;

        void SetTarget(NativeArray<Entity> bees, NativeArray<Entity> targetBees)
        {
            bool targetBeesExist = targetBees.Length != 0;
            bool foodExists = food.Length != 0;
            for (int i = 0; i < bees.Length; i++)
            {
                aggressive = tempTFS.rnd.NextBool();
                Entity bee = bees[i];
                Bee beeComponent = tempTFS._beeComponent[bee];
                
                if (beeComponent.state == BeeState.Idle)
                {

                    if (targetBeesExist && (!foodExists || aggressive)) //Checks if blue bees exist to attack, if so then check if aggressive or no food exists, then attack anyway.
                    {
                        beeComponent.target = yellowBees[tempTFS.rnd.NextInt(yellowBees.Length)];
                        beeComponent.state = BeeState.Attacking;
                        ecb.SetComponent(bee, beeComponent);
                    }
                    else if (foodExists && (!aggressive || !targetBeesExist)) //Checks if not aggressivee or no blue bees to attack, then collect food if it exists
                    {
                        beeComponent.target = food[tempTFS.rnd.NextInt(food.Length)];
                        beeComponent.state = BeeState.Collecting;
                        ecb.SetComponent(bee, beeComponent);
                    }
                }

            }
        }
        
        SetTarget(blueBees, yellowBees);
        SetTarget(yellowBees, blueBees);

        yellowBees.Dispose();
        blueBees.Dispose();
        food.Dispose();
    }

}



