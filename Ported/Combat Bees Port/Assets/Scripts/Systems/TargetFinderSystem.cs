using System.Security.Cryptography;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
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

            EntityQuery _yellowBeeQuery = GetEntityQuery(typeof(YellowTeam));
            _yellowBees = _yellowBeeQuery.ToEntityArray(Allocator.Temp);
            
            EntityQuery _blueBeesQuery = GetEntityQuery(typeof(BlueTeam));
            _blueBees = _blueBeesQuery.ToEntityArray(Allocator.Temp);
            
            EntityQuery _foodQuery = GetEntityQuery(typeof(Food));
            _food = _foodQuery.ToEntityArray(Allocator.Temp);
            
            //Set Targets for all Blue bees.
            Entities.WithAll<YellowTeam>().ForEach((ref Bee bee) =>
            {

                aggression = Random.Range(0, 2);

                if (aggression == 1)
                {
                    bee.target = _blueBees[Random.Range(0, _blueBees.Length)];
                    bee.state = BeeState.Attacking;
                }

                if (aggression == 0)
                {
                    bee.target = _food[Random.Range(0, _food.Length)];
                    bee.state = BeeState.Collecting;
                }
                
            }).Schedule();
            
            //Set Targets for all Blue bees.
            Entities.WithAll<BlueTeam>().ForEach((ref Bee bee) =>
            {

                aggression = Random.Range(0, 2);

                if (aggression == 1)
                {
                    bee.target = _blueBees[Random.Range(0, _yellowBees.Length)];
                    bee.state = BeeState.Attacking;
                }

                if (aggression == 0)
                {
                    bee.target = _food[Random.Range(0, _food.Length)];
                    bee.state = BeeState.Collecting;
                }
                
            }).Schedule();

            _blueBees.Dispose();
            _yellowBees.Dispose();
            _food.Dispose();
        
    }
}
