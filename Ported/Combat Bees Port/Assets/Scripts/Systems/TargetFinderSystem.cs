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

partial struct TargetFinderSystem : ISystem
{
    private EntityQuery _foodQuery;
    private EntityQuery _yellowQuery;
    private EntityQuery _blueQuery;

    bool _foodExists;
    bool _yellowBeeExists;
    bool _blueBeeExists;

    Bee _beeComponent;
    Random rnd;

   
    public void OnCreate(ref SystemState state) { }
    public void OnDestroy(ref SystemState state)
    {
        throw new NotImplementedException();
    }

    public void OnUpdate(ref SystemState state)
    {
        rnd = Random.CreateFromIndex(state.GlobalSystemVersion);

        bool aggressive;
        var allocator = state.WorldUnmanaged.UpdateAllocator.ToAllocator;
        {
            var yellowQueryBuilder = new EntityQueryDescBuilder();
            yellowQueryBuilder.AddAll(ComponentType.ReadWrite<YellowTeam>());
            _yellowQuery = state.GetEntityQuery(yellowQueryBuilder);

            var blueQueryBuilder = new EntityQueryDescBuilder();
            blueQueryBuilder.AddAll(ComponentType.ReadWrite<BlueTeam>());
            _blueQuery = state.GetEntityQuery(blueQueryBuilder);

            var foodQueryBuilder = new EntityQueryDescBuilder();
            foodQueryBuilder.AddAll(ComponentType.ReadWrite<Food>());
            _foodQuery = state.GetEntityQuery(foodQueryBuilder);

            NativeArray<Entity> yellowBees = _yellowQuery.ToEntityArray(allocator);
            NativeArray<Entity> blueBees = _blueQuery.ToEntityArray(allocator);
            NativeArray<Entity> food = _foodQuery.ToEntityArray(allocator);



            _foodExists = food.Length != 0;
            _yellowBeeExists = food.Length != 0;
            _blueBeeExists = blueBees.Length != 0;

            if (_yellowBeeExists) //Makes sure there are yellow Bees
            {

                for (int i = 0; i < yellowBees.Length; i++)
                {

                    aggressive = rnd.NextBool();
                    Entity bee = yellowBees[i];
                    _beeComponent = SystemAPI.GetComponent<Bee>(bee);


                    if (_beeComponent.state == BeeState.Idle)
                    {

                        if (_blueBeeExists && (!_foodExists || aggressive)) //Checks if blue bees exist to attack, if so then check if aggressive or no food exists, then attack anyway.
                        {
                            _beeComponent.target = blueBees[rnd.NextInt(blueBees.Length)];
                            _beeComponent.state = BeeState.Attacking;
                        }
                        else if (_foodExists && (!aggressive || !_blueBeeExists)) //Checks if not aggressivee or no blue bees to attack, then collect food if it exists
                        {
                            _beeComponent.target = blueBees[rnd.NextInt(blueBees.Length)];
                            _beeComponent.state = BeeState.Hauling;
                        }

                    }
                }
            }


            if (_blueBeeExists)
            {


                for (int i = 0; i < blueBees.Length; i++)
                {

                    aggressive = rnd.NextBool();
                    Entity bee = blueBees[i];
                    _beeComponent = SystemAPI.GetComponent<Bee>(bee);


                    if (_beeComponent.state == BeeState.Idle)
                    {

                        if (_blueBeeExists && (!_foodExists || aggressive)) //Checks if blue bees exist to attack, if so then check if aggressive or no food exists, then attack anyway.
                        {
                            _beeComponent.target = yellowBees[rnd.NextInt(yellowBees.Length)];
                            _beeComponent.state = BeeState.Attacking;
                        }
                        else if (_foodExists && (!aggressive || !_blueBeeExists)) //Checks if not aggressivee or no blue bees to attack, then collect food if it exists
                        {
                            _beeComponent.target = yellowBees[rnd.NextInt(yellowBees.Length)];
                            _beeComponent.state = BeeState.Hauling;
                        }

                    }


                }
            }

            yellowBees.Dispose();
            blueBees.Dispose();
            food.Dispose();
        }
    }
}



