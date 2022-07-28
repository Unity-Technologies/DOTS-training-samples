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

    bool _foodExists;
    bool _yellowBeeExists;
    bool _blueBeeExists;

    ComponentDataFromEntity<Bee> _beeComponent;
    Random rnd;


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
        foodQueryBuilder.AddAll(ComponentType.ReadWrite<Food>());
        foodQueryBuilder.FinalizeQuery();
        _foodQuery = state.GetEntityQuery(foodQueryBuilder);

        _beeComponent = state.GetComponentDataFromEntity<Bee>(false);
    }

    public void OnDestroy(ref SystemState state)
    {
        
    }


    public void OnUpdate(ref SystemState state)
    {
        _beeComponent.Update(ref state);

        rnd = Random.CreateFromIndex(state.GlobalSystemVersion);
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        bool aggressive;
        var allocator = state.WorldUnmanaged.UpdateAllocator.ToAllocator;

        NativeArray<Entity> yellowBees = _yellowQuery.ToEntityArray(allocator);
        NativeArray<Entity> blueBees = _blueQuery.ToEntityArray(allocator);
        NativeArray<Entity> food = _foodQuery.ToEntityArray(allocator);



        _foodExists = food.Length != 0;
        _yellowBeeExists = food.Length != 0;
        _blueBeeExists = blueBees.Length != 0;

        for (int i = 0; i < yellowBees.Length; i++)
        {
            aggressive = rnd.NextBool();
            Entity bee = yellowBees[i];
            Bee beeComponent = _beeComponent[bee];




            if (beeComponent.state == BeeState.Idle)
            {

                if (_blueBeeExists && (!_foodExists || aggressive)) //Checks if blue bees exist to attack, if so then check if aggressive or no food exists, then attack anyway.
                {
                    beeComponent.target = blueBees[rnd.NextInt(blueBees.Length)];
                    beeComponent.state = BeeState.Attacking;
                    ecb.SetComponent(bee, beeComponent);
                }
                else if (_foodExists && (!aggressive || !_blueBeeExists)) //Checks if not aggressivee or no blue bees to attack, then collect food if it exists
                {
                    beeComponent.target = food[rnd.NextInt(food.Length)];
                    beeComponent.state = BeeState.Collecting;
                    ecb.SetComponent(bee, beeComponent);
                }

            }
        }




        foreach (Entity bee in blueBees)
        {
            aggressive = rnd.NextBool();
            Bee beeComponent = _beeComponent[bee];



            if (beeComponent.state == BeeState.Idle)
            {

                if (_yellowBeeExists && (!_foodExists || aggressive)) //Checks if blue bees exist to attack, if so then check if aggressive or no food exists, then attack anyway.
                {
                    beeComponent.target = yellowBees[rnd.NextInt(yellowBees.Length)];
                    beeComponent.state = BeeState.Attacking;
                    ecb.SetComponent(bee, beeComponent);
                }
                else if (_foodExists && (!aggressive || !_yellowBeeExists)) //Checks if not aggressivee or no blue bees to attack, then collect food if it exists
                {
                    beeComponent.target = food[rnd.NextInt(food.Length)];
                    beeComponent.state = BeeState.Collecting;
                    ecb.SetComponent(bee, beeComponent);
                }
            }


        }


        yellowBees.Dispose();
        blueBees.Dispose();
        food.Dispose();
    }

}



