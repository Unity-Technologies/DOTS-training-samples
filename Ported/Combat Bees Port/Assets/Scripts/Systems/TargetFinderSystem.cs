using System.Security.Cryptography;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Random = UnityEngine.Random;

partial class TargetFinderSystem : SystemBase
{
    [BurstCompile]
    protected override void OnUpdate()
    {
            NativeArray<Entity> _yellowBees;
            NativeArray<Entity> _blueBees;
            NativeArray<Entity> _food;
            int aggression = 0;

            var allocator = World.UpdateAllocator.ToAllocator;
            
            EntityQuery _yellowBeeQuery = GetEntityQuery(typeof(YellowTeam));
            _yellowBees = _yellowBeeQuery.ToEntityArray(allocator);
            
            EntityQuery _blueBeesQuery = GetEntityQuery(typeof(BlueTeam));
            _blueBees = _blueBeesQuery.ToEntityArray(allocator);
            
            EntityQuery _foodQuery = GetEntityQuery(typeof(Food));
            _food = _foodQuery.ToEntityArray(allocator);
            
            //Set Targets for all Blue bees.
            Entities.WithAll<YellowTeam>().ForEach((ref Bee bee) =>
            {

                aggression = Random.Range(0, 2);

                if (aggression == 1)
                {
                    Entity target = _blueBees[Random.Range(0, _blueBees.Length)];
                    bee.target = target;
                    bee.state = BeeState.Attacking;
                }

                if (aggression == 0 && !(_food[0] == null))
                {  
                    Entity target = _food[Random.Range(0, _food.Length)];
                    bee.target = target;
                    bee.state = BeeState.Collecting;
                }
                
                
            }).Schedule();
            
            //Set Targets for all Yellow bees.
            Entities.WithAll<BlueTeam>().ForEach((ref Bee bee) =>
            {

                aggression = Random.Range(0, 2);

                if (aggression == 1 && !(_yellowBees[0] == null))
                {
                    Entity target = _yellowBees[Random.Range(0, _yellowBees.Length)];
                    bee.target = target;
                    bee.state = BeeState.Attacking;
                    
                }

                if (aggression == 0 && !(_food[0] == null))
                {
                    Entity target = _food[Random.Range(0, _food.Length)];
                    bee.target = target;
                    bee.state = BeeState.Collecting;
                }
                
               
                
            }).Schedule();
    }
}
