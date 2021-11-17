using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

using UnityEngine;

public partial class PathingSystem : SystemBase
{
    EntityQuery beeQuery;
    EntityQuery foodQuery;

    const float startHuntingDistance = 2.0f;
    const float stopHuntingDistance = 3.0f;

    Unity.Mathematics.Random random = new Unity.Mathematics.Random(85834);

    protected override void OnCreate()
    {
        beeQuery = GetEntityQuery(ComponentType.ReadOnly<Bee>());
        foodQuery = GetEntityQuery(ComponentType.ReadOnly<Food>());
    }

    protected override void OnUpdate()
    {
        var bees = beeQuery.ToEntityArray(Allocator.Temp);
        var food = foodQuery.ToEntityArray(Allocator.Temp);
        var entityPositions = GetComponentDataFromEntity<Translation>(true);
        var entityVelocity = GetComponentDataFromEntity<Velocity>(true);
        var teams = GetComponentDataFromEntity<TeamID>(true);
        var beeData = GetComponentDataFromEntity<Bee>(false);
        var foodData = GetComponentDataFromEntity<Food>(false);

        // Grab Food
        Entities
            .WithNone<Gravity>()
            .WithStructuralChanges()
            .ForEach((Entity entity, ref Translation translation, ref Food food) =>
        {
            if (food.CarriedBy == Entity.Null)
            {
                for (int i = 0; i < bees.Length; ++i)
                {
                    Entity ithBee = bees[i];
                    var distanceSqr = math.lengthsq(translation.Value - entityPositions[ithBee].Value);
                    if (distanceSqr < 1.0f)
                    {
                        food.CarriedBy = ithBee;
                        beeData[ithBee] = new Bee { Carried = entity, Mode = Bee.ModeCategory.Returning };
                        break;
                    }
                }
            }
        }).Run();


        var globals = GetSingletonEntity<Globals>();
        var globalsComponent = GetComponent<Globals>(globals);

        // Kill a Nearby Bee 
        Entities
            .WithNone<Gravity>()
            .WithStructuralChanges()
            .ForEach((Entity entity, ref Translation translation, ref Bee bee, in TeamID teamID) =>
        {
            if (bee.Mode == Bee.ModeCategory.Hunting)
            {
                for (int i = 0; i < bees.Length; ++i)
                {
                    Entity ithBee = bees[i];
                    var distanceSqr = math.lengthsq(translation.Value - entityPositions[ithBee].Value);
                    if (distanceSqr < 1.0f && teams[ithBee].Value != teamID.Value)
                    {
                        EntityManager.AddComponentData(ithBee, new Gravity());

                        int totalGiblets = random.NextInt(5, 10);
                        for (int j = 0; j < totalGiblets; ++j)
                        {
                            var giblet = EntityManager.Instantiate(globalsComponent.GibletPrefab);
                            EntityManager.SetComponentData<Translation>(giblet, translation);
                            EntityManager.SetComponentData<Velocity>(giblet, new Velocity
                            {
                                Value = entityVelocity[ithBee].Value + random.NextFloat3Direction() * 2.0f
                            });
                        }

                        break;
                    }
                }
            }
        }).Run();

        Entities
            .ForEach((ref Bee bee, ref Goal goal, ref Translation translation, in TeamID team) =>
        {
            if (bee.Mode == Bee.ModeCategory.Returning)
            {
                if (team.Value == 0)
                {
                    goal.target = new float3(-18, 0, translation.Value.z);

                    if (translation.Value.x < -15)
                    {
                        bee.Mode = Bee.ModeCategory.Searching;
                    }
                }
                else
                {
                    goal.target = new float3(18, 0, translation.Value.z);
                    if (translation.Value.x > 15)
                    {
                        bee.Mode = Bee.ModeCategory.Searching;
                    }
                }
            }
            else 
            {
                float huntingDistance = (bee.Mode == Bee.ModeCategory.Hunting) ? stopHuntingDistance : startHuntingDistance;
                float huntingDistanceSq = huntingDistance * huntingDistance;

                bee.Mode = Bee.ModeCategory.Searching;
                float closestDistanceSqr = float.MaxValue;
                for (int i = 0; i < bees.Length; ++i)
                {
                    Entity ithBee = bees[i];
                    var distanceSqr = math.lengthsq(translation.Value - entityPositions[ithBee].Value);
                    if (distanceSqr < huntingDistanceSq && teams[ithBee].Value != team.Value)
                    {
                        //closestBee = ithBee;
                        closestDistanceSqr = distanceSqr;
                        goal.target = entityPositions[ithBee].Value;
                        bee.Mode = Bee.ModeCategory.Hunting;
                    }
                }

                if (bee.Mode == Bee.ModeCategory.Searching)
                {
                    closestDistanceSqr = float.MaxValue;
                    for (int i = 0; i < food.Length; ++i)
                    {
                        Entity ithFood = food[i];
                        if (foodData[ithFood].CarriedBy != Entity.Null
                            || entityPositions[ithFood].Value.x < -12.5
                            || entityPositions[ithFood].Value.x > 12.5)
                        {
                            continue;
                        }

                        var distanceSqr = math.lengthsq(translation.Value - entityPositions[ithFood].Value);
                        if (distanceSqr < closestDistanceSqr)
                        {
                            closestDistanceSqr = distanceSqr;
                            goal.target = entityPositions[ithFood].Value;
                        }
                    }
                }
            }
        }).Run();

        bees.Dispose();
        food.Dispose();
    }
}
