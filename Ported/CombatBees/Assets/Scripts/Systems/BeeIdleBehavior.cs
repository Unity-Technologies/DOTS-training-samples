using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;
using Random = Unity.Mathematics.Random;

public partial class BeeIdleBehavior : SystemBase
{
    private EntityQuery BeeQuery;
    private EntityQuery FoodQuery;

    protected override void OnCreate()
    {
        BeeQuery = GetEntityQuery(
            typeof(Bee), 
            typeof(Translation), 
            typeof(Velocity), 
            typeof(TeamID), 
            ComponentType.Exclude<Ballistic>(),  
            ComponentType.Exclude<Decay>());
        
        FoodQuery = GetEntityQuery(
            typeof(Food),
            typeof(Translation),
            ComponentType.Exclude<Ballistic>(),  
            ComponentType.Exclude<Decay>());
    }

    protected override void OnUpdate()
    {
        var beeCount = BeeQuery.CalculateEntityCount();
        var foodCount = FoodQuery.CalculateEntityCount();

        var foodPositions = FoodQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        var foodEntities = FoodQuery.ToEntityArray(Allocator.TempJob);
        
        var beePositions = BeeQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        var beeVelocities = BeeQuery.ToComponentDataArray<Velocity>(Allocator.TempJob);
        var beeTeams = BeeQuery.ToComponentDataArray<TeamID>(Allocator.TempJob);
        var beeEntities = BeeQuery.ToEntityArray(Allocator.TempJob);

        var storage = GetStorageInfoFromEntity();

        var frameCount = UnityEngine.Time.frameCount +1;
        
        var dt = Time.DeltaTime;

        Entities
            .WithAll<BeeIdleMode>()
            .WithNone<Ballistic, Decay>()
            .WithDisposeOnCompletion(beePositions)
            .WithDisposeOnCompletion(beeEntities)
            .WithDisposeOnCompletion(beeVelocities)
            .WithDisposeOnCompletion(beeTeams)
            .WithDisposeOnCompletion(foodPositions)
            .WithDisposeOnCompletion(foodEntities)
            .ForEach((Entity entity, int entityInQueryIndex, ref Bee myself, in Translation position, in TeamID team) =>
                {
                    var random = Random.CreateFromIndex((uint)(entityInQueryIndex + frameCount));

                    if (beeCount > 0)
                    {
                        var r = beePositions[0].Value;
                        var e = beeEntities[0];
                        var v = beeVelocities[0].Value;
                        var t = beeTeams[0].Value;
                    }

                    if (foodCount > 0)
                    {
                        var r = foodPositions[0].Value;
                        var e = foodEntities[0];
                    }
                }
            ).Schedule();

    }
}
