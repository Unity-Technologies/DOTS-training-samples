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
using Random = Unity.Mathematics.Random;

partial class TargetFinderSystem : SystemBase
{
    [BurstCompile]
    protected override void OnUpdate()
    {
            NativeArray<Entity> _yellowBees;
            NativeArray<Entity> _blueBees;
            NativeArray<Entity> _food;
            Random rnd = Random.CreateFromIndex((uint)Time.ElapsedTime);
            bool  aggression = rnd.NextBool();




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


                if (aggression && !(_blueBees[0] == null))
                {
                    Entity target = _blueBees[rnd.NextInt()];
                    bee.target = target;
                    bee.state = BeeState.Attacking;
                }

                if (!aggression && !(_food[0] == null))
                {  
                    Entity target = _food[rnd.NextInt()];
                    bee.target = target;
                    bee.state = BeeState.Collecting;
                }
                
                
            }).Schedule();
            
            //Set Targets for all Yellow bees.
            Entities.WithAll<BlueTeam>().ForEach((ref Bee bee) =>
            {

                

                if (aggression && !(_yellowBees[0] == null))
                {
                    
                    Entity target = _yellowBees[rnd.NextInt()];
                    bee.target = target;
                    bee.state = BeeState.Attacking;
                    
                }

                if (!aggression && !(_food[0] == null))
                {
                    Entity target = _food[rnd.NextInt()];
                    bee.target = target;
                    bee.state = BeeState.Collecting;
                }
                
            
                
            }).Schedule();
    }
}
