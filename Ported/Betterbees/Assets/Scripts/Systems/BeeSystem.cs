using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct BeeSystem : ISystem
{
    private EntityQuery _availableFoodSourcesQuery;
    private uint _updateCounter;
    private float3 _halfFloat;

    private NativeArray<float3> _boundaryNormals;
    private static readonly int _boundaryNormalCount = 3;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _halfFloat = new float3(0.5f, 0.5f, 0.5f);
        
        var builder = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<FoodComponent>()
            .WithNone<Parent>();

        _availableFoodSourcesQuery = state.GetEntityQuery(builder);

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
            foreach (var (beeState, transform, velocity, target, entity) in SystemAPI
                .Query<RefRW<BeeState>, RefRW<LocalTransform>, RefRW<VelocityComponent>, RefRW<TargetComponent>>()
                .WithEntityAccess())
            {
                BaseMovement(velocity, ref random, SystemAPI.Time.DeltaTime, ref beeSettings);

                switch (beeState.ValueRO.state)
                {
                    case BeeState.State.IDLE:
                        Idle(beeState);
                        break;
                    case BeeState.State.GATHERING:
                        Gathering(entity, beeState, transform, velocity, target, ref state, random, commandBuffer, ref beeSettings);
                        break;
                    case BeeState.State.ATTACKING:
                        Attacking();
                        break;
                    case BeeState.State.RETURNING:
                        Returning();
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

        if (dist2 > beeSettings.interactionDistanceSquared)
        {
            velocity.ValueRW.Velocity += delta * (beeSettings.chaseForce * SystemAPI.Time.DeltaTime / math.sqrt(dist2));
        }
        else
        {
            commandBuffer.AddComponent(target.ValueRO.Target, new Parent { Value = beeEntity });

            targetTransform.Position = new float3();
            state.EntityManager.SetComponentData(target.ValueRO.Target, targetTransform);

            beeState.ValueRW.state = BeeState.State.RETURNING;
            target.ValueRW.Target = Entity.Null;
        }
    }

    private void Attacking()
    {

    }

    private void Returning()
    {

    }
}