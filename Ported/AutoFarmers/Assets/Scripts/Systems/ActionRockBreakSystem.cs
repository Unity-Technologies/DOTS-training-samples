using System.Resources;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
/*
[BurstCompile]
public partial struct ActionRockBreakSystem : ISystem
{
    EntityQuery rockQuery;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Ground>();
        rockQuery = state.EntityManager.CreateEntityQuery(typeof(Rock));
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<GameConfig>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        int rockEntityCount = rockQuery.CalculateEntityCount();

        BufferFromEntity<GroundTile> groundData = state.GetBufferFromEntity<GroundTile>(false);
        Entity groundEntity = SystemAPI.GetSingletonEntity<Ground>();

        if (!groundData.TryGetBuffer(groundEntity, out DynamicBuffer<GroundTile> groundBufferData))
        {
            return;
        }

        foreach (FarmerRockBreakingAspect instance in SystemAPI.Query<FarmerRockBreakingAspect>())
        {
            FarmerIntentState intentState = instance.intent.value;
            if (intentState != FarmerIntentState.SmashRocks) { continue; }

            ProcessFarmerInstance(instance, ref groundBufferData, config, ref state, ref ecb, rockEntityCount);
        }
    }


    void ProcessFarmerInstance(in FarmerRockBreakingAspect instance, ref DynamicBuffer<GroundTile> groundBufferData, in GameConfig config, ref SystemState state, ref EntityCommandBuffer ecb, in int rockEntityCount)
    {
        bool needsDestination = instance.pathfindingIntent.destinationType == PathfindingDestination.None;
        bool hasTarget = instance.combat.combatTarget != Entity.Null;

        if (hasTarget)
        {
            if (!state.EntityManager.Exists(instance.combat.combatTarget))
            {
                ecb.SetComponent(instance.Self, CreateEmptyIntent(instance.intent.random));
            }
            else
            {
                float newCooldownTicker = instance.combat.cooldownTicker - state.Time.DeltaTime;
                if (newCooldownTicker <= 0)
                {
                    if (TryBreakRock(instance.combat.combatTarget, config.RockDamagePerHit, ref state, ref ecb))
                    {
                        GroundUtilities.DestroyRock(instance.combat.combatTarget, state.EntityManager, ecb, config, ref groundBufferData);

                        instance.combat = new FarmerCombat
                        {
                            combatTarget = Entity.Null,
                            cooldownTicker = 0.0f
                        };
                        ecb.SetComponent(instance.Self, CreateEmptyIntent(instance.intent.random));
                    }
                    else
                    {
                        instance.combat = new FarmerCombat
                        {
                            combatTarget = instance.combat.combatTarget,
                            cooldownTicker = config.FarmerAttackCooldown
                        };
                    }
                }
                else
                {
                    instance.combat = new FarmerCombat
                    {
                        cooldownTicker = newCooldownTicker
                    };
                }
            }
        }
        else if (needsDestination)
        {
            if (rockEntityCount > 0)
            {
                ecb.SetComponent(instance.Self, new PathfindingIntent
                {
                    navigatorType = NavigatorType.Farmer,
                    destinationType = PathfindingDestination.Rock,
                    RequiredZone = GroundUtilities.GetFullMapBounds(config)
                });
            }
            else
            {
                ecb.SetComponent(instance.Self, CreateEmptyIntent(instance.intent.random));
            }
        }
        else if (IsInRangeOfPathfindingDestination(instance.translation, instance.PathfindingWaypoints, config.RockSmashActionRange, config.MapSize.x))
        {
            if (instance.PathfindingWaypoints.Length > 0)
            {
                Waypoint destination = instance.PathfindingWaypoints.ElementAt(0);
                Entity rockEntity = groundBufferData[destination.TileIndex].rockEntityByTile;
                if (rockEntity != Entity.Null)
                {
                    instance.combat = new FarmerCombat
                    {
                        combatTarget = rockEntity,
                        cooldownTicker = 0.0f
                    };
                }
                else
                {
                    ecb.SetComponent(instance.Self, CreateEmptyIntent(instance.intent.random));
                }
            }
            else
            {
                ecb.SetComponent(instance.Self, CreateEmptyIntent(instance.intent.random));
            }
        }
        else
        {
            // Wait

            instance.combat = new FarmerCombat
            {
                combatTarget = instance.combat.combatTarget,
                cooldownTicker = instance.combat.cooldownTicker
            };
        }
    }

    bool IsInRangeOfPathfindingDestination(in Translation translation, in DynamicBuffer<Waypoint> waypoints, in float rockBreakDist, in int mapWidth)
    {
        if (waypoints.Length == 0) return true;

        Waypoint destination = waypoints.ElementAt(0);

        float2 finalWaypointTranslation = GroundUtilities.GetTileTranslation(destination.TileIndex, mapWidth);

        float rockDistanceSquared = math.distancesq(finalWaypointTranslation, translation.Value.xz);

        return rockDistanceSquared < rockBreakDist * rockBreakDist;
    }
    
    bool TryBreakRock(in Entity rockEntity, in float damagePerHit, ref SystemState state, ref EntityCommandBuffer ecb)
    {
        RockHealth rockHealth = state.EntityManager.GetComponentData<RockHealth>(rockEntity);
        float newHealth = rockHealth.Value - damagePerHit;
        bool didBreak = newHealth <= 0;

        ecb.SetComponent(rockEntity, new RockHealth
        {
            Value = newHealth
        });

        return didBreak;
    }

    float2 CalcRockClosestPoint(in Translation translation, in Translation rockTranslation, in Rock rock)
    {
        float3 size = rock.size;

        return math.clamp(
            translation.Value.xz,
            rockTranslation.Value.xz - size.xz / 2,
            rockTranslation.Value.xz + size.xz / 2);
    }

    static FarmerIntent CreateEmptyIntent(in Random random)
    {
        return new FarmerIntent
        {
            value = FarmerIntentState.None,
            random = random,
            elapsed = 0
        };
    }
}
*/