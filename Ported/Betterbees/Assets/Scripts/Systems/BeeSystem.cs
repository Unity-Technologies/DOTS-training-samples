using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEditor;
using static UnityEngine.GraphicsBuffer;


[BurstCompile]
public partial struct BeeSystem : ISystem
{
    private EntityQuery _availableFoodSourcesQuery;
    private EntityQuery _availableBeesQuery;
    private uint _updateCounter;
    private float3 _halfFloat;

    private NativeArray<float3> _boundaryNormals;
    private static readonly int _boundaryNormalCount = 3;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _halfFloat = new float3(0.5f, 0.5f, 0.5f);
        {
            var builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<FoodComponent>()
                .WithNone<Parent, GravityComponent>();

            _availableFoodSourcesQuery = state.GetEntityQuery(builder);
        }
        {
            var builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<BeeState>();
            _availableBeesQuery = state.GetEntityQuery(builder);
        }

        _boundaryNormals = new NativeArray<float3>(_boundaryNormalCount, Allocator.Persistent);
        for (int i = 0; i < _boundaryNormalCount; i++)
        {
            float3 normal = float3.zero;
            normal[i] = 1;
            _boundaryNormals[i] = normal;
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        if (_boundaryNormals.IsCreated)
            _boundaryNormals.Dispose();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        var beeSettings = SystemAPI.GetSingleton<BeeSettingsSingletonComponent>();
        var random = Random.CreateFromIndex(_updateCounter++);

        using (var commandBuffer = new EntityCommandBuffer(Allocator.TempJob))
        {
            foreach (var (beeState, transform, velocity, target, home, entity) in SystemAPI
                .Query<RefRW<BeeState>, RefRW<LocalTransform>, RefRW<VelocityComponent>, RefRW<TargetComponent>, RefRO<ReturnHomeComponent>>()
                .WithEntityAccess())
            {
                BaseMovement(velocity, ref random, SystemAPI.Time.DeltaTime, ref beeSettings);

                switch (beeState.ValueRO.state)
                {
                    case BeeState.State.IDLE:
                        Idle(beeState);
                        break;
                    case BeeState.State.GATHERING:
                        float agression = random.NextFloat();
                        if (agression < beeSettings.agressionPercentage)
                        {
                            target.ValueRW.Target = Entity.Null;
                            beeState.ValueRW.state = BeeState.State.ATTACKING;
                        }
                        else
                        {
                            Gathering(entity, beeState, transform, velocity, target, ref state, random, commandBuffer, ref beeSettings);
                        }
                        break;
                    case BeeState.State.ATTACKING:

                        Attacking(entity, beeState, transform, velocity, target, ref state, random, commandBuffer, ref beeSettings);
                        break;
                    case BeeState.State.RETURNING:
                        Returning(entity, beeState, transform, velocity, target, home, ref beeSettings, ref state, commandBuffer);
                        break;
                }

                ApplyBoundaries(config, transform.ValueRO.Position, velocity);
            }

            commandBuffer.Playback(state.EntityManager);
        }
    }

    private void ApplyBoundaries(
        in Config config,
        float3 position,
        RefRW<VelocityComponent> velocity)
    {
        float3 boundsNormal = float3.zero;
        bool outsideBounds = false;
        for (int i = 0; i < _boundaryNormalCount; i++)
        {
            if (position[i] > config.bounds[i])
            {
                boundsNormal += -_boundaryNormals[i];
                outsideBounds = true;
            }
            else if (position[i] < -config.bounds[i])
            {
                boundsNormal += _boundaryNormals[i];
                outsideBounds = true;
            }
        }
        boundsNormal = math.normalize(boundsNormal);

        if (outsideBounds)
        {
            velocity.ValueRW.Velocity = boundsNormal;
        }
    }

    private void BaseMovement(
        RefRW<VelocityComponent> velocity,
        ref Random random,
        float dt,
        ref BeeSettingsSingletonComponent beeSettings)
    {
        velocity.ValueRW.Velocity += (random.NextFloat3() - _halfFloat) * beeSettings.flightJitter * dt;
        velocity.ValueRW.Velocity *= (1f - beeSettings.damping);
    }

    private void Idle(RefRW<BeeState> beeState)
    {
        beeState.ValueRW.state = BeeState.State.GATHERING;
    }

    private void Gathering(
        Entity beeEntity,
        RefRW<BeeState> beeState,
        RefRW<LocalTransform> transform,
        RefRW<VelocityComponent> velocity,
        RefRW<TargetComponent> target,
        ref SystemState state,
        Random random,
        EntityCommandBuffer commandBuffer,
        ref BeeSettingsSingletonComponent beeSettings)
    {
        RemoveInvalidTarget(target);
        FindFoodTarget(target, beeState, ref random);
        FlyToTarget(beeEntity, beeState, transform, velocity, target, ref state, commandBuffer, ref beeSettings);
    }

    private void RemoveInvalidTarget(RefRW<TargetComponent> target)
    {
        bool hasInvalidTarget = target.ValueRO.Target != Entity.Null && !_availableFoodSourcesQuery.Matches(target.ValueRO.Target);
        if (hasInvalidTarget)
        {
            target.ValueRW.Target = Entity.Null;
        }
    }

    private void FindFoodTarget(RefRW<TargetComponent> target, RefRW<BeeState> beeState, ref Random random)
    {
        bool alreadyHasTarget = target.ValueRO.Target != Entity.Null;
        if (alreadyHasTarget)
        {
            return;
        }

        var foodEntities = _availableFoodSourcesQuery.ToEntityArray(Allocator.Temp);

        if (foodEntities.Length > 0)
        {
            int index = (int)(random.NextUInt() % foodEntities.Length);
            target.ValueRW.Target = foodEntities[index];
        }
        else
        {
            beeState.ValueRW.state = BeeState.State.IDLE;
        }
    }

    private void FlyToTarget(
        Entity beeEntity,
        RefRW<BeeState> beeState,
        RefRW<LocalTransform> transform,
        RefRW<VelocityComponent> velocity,
        RefRW<TargetComponent> target,
        ref SystemState state,
        EntityCommandBuffer commandBuffer,
        ref BeeSettingsSingletonComponent beeSettings)
    {
        if (target.ValueRO.Target == Entity.Null)
        {
            return;
        }

        var targetTransform = state.EntityManager.GetComponentData<LocalTransform>(target.ValueRO.Target);
        float3 delta = targetTransform.Position - transform.ValueRO.Position;
        float dist2 = delta.x * delta.x + delta.y * delta.y + delta.z * delta.z;

        if (dist2 > beeSettings.interactionDistance * beeSettings.interactionDistance)
        {
            float dist = math.sqrt(dist2);
            velocity.ValueRW.Velocity += delta * (beeSettings.chaseForce * SystemAPI.Time.DeltaTime / math.sqrt(dist));
        }
        else
        {
            commandBuffer.AddComponent(target.ValueRO.Target, new Parent { Value = beeEntity });

            targetTransform.Position = new float3();
            commandBuffer.SetComponent(target.ValueRO.Target, targetTransform);

            beeState.ValueRW.state = BeeState.State.RETURNING;
        }
    }

    private void Attacking(
        Entity beeEntity,
        RefRW<BeeState> beeState, 
        RefRW<LocalTransform> transform,
        RefRW<VelocityComponent> velocity, 
        RefRW<TargetComponent> target,
        ref SystemState state,
        Random random,
        EntityCommandBuffer commandBuffer,
        ref BeeSettingsSingletonComponent beeSettings)
    {
        FindBeeTarget(target, beeState, ref random, ref state);
        // if the beeTarget is not valid
            // set the status to idle
        // Speed up and Move towards beeTarget
        // check distance to the target (hitRadius)
            // if (dist < hitRadius)
                // destroy the beeTarget
                // set back status to idle
    }

    private void RemoveInvalidBeeTarget(RefRW<TargetComponent> target)
    {
        bool hasInvalidTarget = target.ValueRO.Target != Entity.Null && !_availableBeesQuery.Matches(target.ValueRO.Target);
        if (hasInvalidTarget)
        {
            target.ValueRW.Target = Entity.Null;
        }
    }

    private void FindBeeTarget(RefRW<TargetComponent> target, RefRW<BeeState> beeState, ref Random random, ref SystemState state)
    {
        bool alreadyHasTarget = target.ValueRO.Target != Entity.Null;
        if (alreadyHasTarget)
        {
            return;
        }

        var beeEntities = _availableBeesQuery.ToEntityArray(Allocator.Temp);

        if (beeEntities.Length > 0)
        {
            int index = (int)(random.NextUInt() % beeEntities.Length);
            var targetEntity = beeEntities[index];
            var beeTargetState = state.EntityManager.GetComponentData<BeeState>(targetEntity);
            if (beeTargetState.hiveId == beeState.ValueRO.hiveId)
            {
                return; // if all enemies are dead we are stuck in attack /!\
            }
            target.ValueRW.Target = targetEntity;
        }
        else
        {
            beeState.ValueRW.state = BeeState.State.IDLE;
        }
    }

    private void Returning(
        Entity beeEntity,
        RefRW<BeeState> beeState,
        RefRW<LocalTransform> transform,
        RefRW<VelocityComponent> velocity,
        RefRW<TargetComponent> target,
        RefRO<ReturnHomeComponent> home,
        ref BeeSettingsSingletonComponent beeSettings,
        ref SystemState state,
        EntityCommandBuffer commandBuffer)
    {
        float3 beePos = transform.ValueRO.Position;
        float3 targetPos = math.clamp(beePos, home.ValueRO.HomeMinBounds, home.ValueRO.HomeMaxBounds);
        float3 delta = targetPos - beePos;
        float dist2 = delta.x * delta.x + delta.y * delta.y + delta.z * delta.z;

        if (dist2 > beeSettings.interactionDistance * beeSettings.interactionDistance)
        { 
            float dist = math.sqrt(dist2);
            velocity.ValueRW.Velocity += delta * (beeSettings.carryForce * SystemAPI.Time.DeltaTime / math.sqrt(dist));
        }
        else
        {
            Entity foodEntity = target.ValueRO.Target;

            commandBuffer.RemoveComponent<Parent>(foodEntity);

            var foodTransform = state.EntityManager.GetComponentData<LocalTransform>(foodEntity);
            foodTransform.Position = transform.ValueRO.Position;
            commandBuffer.SetComponent(foodEntity, foodTransform);

            commandBuffer.SetComponent(foodEntity, velocity.ValueRO);

            commandBuffer.AddComponent<GravityComponent>(foodEntity);

            beeState.ValueRW.state = BeeState.State.IDLE;
            target.ValueRW.Target = Entity.Null;
        }
    }
}