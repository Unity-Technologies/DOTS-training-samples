using System;
using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

partial class TargetFinderSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var allocator = World.UpdateAllocator.ToAllocator;
        var rnd = Random.CreateFromIndex(GlobalSystemVersion);

        var yellowBees = GetEntityQuery(typeof(YellowTeam)).ToEntityArray(allocator);

        var blueBees = GetEntityQuery(typeof(BlueTeam)).ToEntityArray(allocator);

        var food = GetEntityQuery(typeof(NotCollected)).ToEntityArray(allocator);

        //Set Targets for all Blue bees.
        Entities.WithAll<YellowTeam>().ForEach((ref Bee bee) =>
        {
            var aggression = rnd.NextBool();
            if (!Exists(bee.target))
            {
                bee.state = BeeState.Idle;
                bee.target = Entity.Null;
            }

            if (aggression && blueBees.Length != 0 && bee.state == BeeState.Idle)
            {
                bee.target = blueBees[rnd.NextInt(blueBees.Length)];
                bee.state = BeeState.Attacking;
            }

            if (!aggression && food.Length != 0 && bee.state == BeeState.Idle)
            {
                bee.target = food[rnd.NextInt(food.Length)];
                bee.state = BeeState.Collecting;
            }

        }).Schedule();
        
        //Set Targets for all Yellow bees.
        Entities.WithAll<BlueTeam>().ForEach((ref Bee bee) =>
        {
            var aggression = rnd.NextBool();
            if (!Exists(bee.target))
            {
                bee.state = BeeState.Idle;
                bee.target = Entity.Null;
            }

            if (aggression && yellowBees.Length != 0 && bee.state == BeeState.Idle)
            {
                bee.target = yellowBees[rnd.NextInt(yellowBees.Length)];
                bee.state = BeeState.Attacking;

            }

            if (!aggression && food.Length != 0 && bee.state == BeeState.Idle)
            {
                bee.target = food[rnd.NextInt(food.Length)];
                bee.state = BeeState.Collecting;
            }

        }).Schedule();
    }
}
