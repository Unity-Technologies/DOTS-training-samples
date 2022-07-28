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

partial class TargetFinderSystem : SystemBase
{

    protected override void OnUpdate()
    {
            NativeArray<Entity> _yellowBees;
            NativeArray<Entity> _blueBees;
            NativeArray<Entity> _food;
            Random rnd = Random.CreateFromIndex(GlobalSystemVersion);
            bool  aggression = rnd.NextBool();


            var allocator = World.UpdateAllocator.ToAllocator;
            
            EntityQuery _yellowBeeQuery = GetEntityQuery(typeof(YellowTeam));
            _yellowBees = _yellowBeeQuery.ToEntityArray(allocator);
            
            EntityQuery _blueBeesQuery = GetEntityQuery(typeof(BlueTeam));
            _blueBees = _blueBeesQuery.ToEntityArray(allocator);
            
            EntityQuery _foodQuery = GetEntityQuery(typeof(NotCollected));
            _food = _foodQuery.ToEntityArray(allocator);

            //Set Targets for all Blue bees.
            Entities.WithAll<YellowTeam>().ForEach((ref Bee bee) =>
            {
                if (!Exists(bee.target))
                {
                    bee.state = BeeState.Idle;
                    bee.target = Entity.Null;
                }
                
                if (aggression && _blueBees.Length != 0 && bee.state == BeeState.Idle)
                {
                    bee.target = _blueBees[rnd.NextInt(_blueBees.Length)];
                    bee.state = BeeState.Attacking;
                }

                if (!aggression && _food.Length != 0 && bee.state == BeeState.Idle)
                {
                    bee.target = _food[rnd.NextInt(_food.Length)];
                    bee.state = BeeState.Collecting;
                }
                
            }).Schedule();
            
            //Set Targets for all Yellow bees.
            Entities.WithAll<BlueTeam>().ForEach((ref Bee bee) =>
            {
                if (!Exists(bee.target))
                {
                    bee.state = BeeState.Idle;
                    bee.target = Entity.Null;
                }

                if (aggression && _yellowBees.Length != 0 && bee.state == BeeState.Idle)
                {
                    bee.target = _yellowBees[rnd.NextInt(_yellowBees.Length)];
                    bee.state = BeeState.Attacking;
                    
                }

                if (!aggression && _food.Length != 0 && bee.state == BeeState.Idle)
                {
                    bee.target = _food[rnd.NextInt(_food.Length)];
                    bee.state = BeeState.Collecting;
                }
                
            }).Schedule();
    }
}
