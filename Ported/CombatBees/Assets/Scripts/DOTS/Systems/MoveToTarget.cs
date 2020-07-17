using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class MoveToTarget : SystemBase
{
    EntityQuery m_TeamOneFieldsQuery;
    EntityQuery m_TeamTwoFieldsQuery;
    EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_TeamOneFieldsQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<TeamOne>(),
                ComponentType.ReadOnly<FieldInfo>()
            }
        });

        m_TeamTwoFieldsQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<TeamTwo>(),
                ComponentType.ReadOnly<FieldInfo>()
            }
        });

        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        var chaseForce = BeeManager.Instance.chaseForce;
        var attackForce = BeeManager.Instance.attackForce;
        var carryForce = BeeManager.Instance.carryForce;
        var attackDistance = BeeManager.Instance.attackDistance;
        var hitDistance = BeeManager.Instance.hitDistance;
        var rotationStiffness = BeeManager.Instance.rotationStiffness;
        var grabDistance = BeeManager.Instance.grabDistance;

        var teamOneFields = m_TeamOneFieldsQuery.ToComponentDataArrayAsync<FieldInfo>(Allocator.TempJob, out var teamOneFieldsHandle);
        var teamTwoFields = m_TeamTwoFieldsQuery.ToComponentDataArrayAsync<FieldInfo>(Allocator.TempJob, out var teamTwoFieldsHandle);

        Dependency = JobHandle.CombineDependencies(Dependency, teamOneFieldsHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, teamTwoFieldsHandle);

        var ecb = m_ECBSystem.CreateCommandBuffer().ToConcurrent();

        Entities
            .WithNone<DespawnTimer>()
            .WithDeallocateOnJobCompletion(teamOneFields)
            .WithDeallocateOnJobCompletion(teamTwoFields)
            .ForEach((Entity entity, int entityInQueryIndex, ref Velocity velocity, ref Smoothing smoothing, ref Target target, in Translation pos) =>
            {
                var oldSmoothPos = smoothing.SmoothPosition;

                Translation targetPos;
                if (target.EnemyTarget != Entity.Null)
                {
                    targetPos = GetComponent<Translation>(target.EnemyTarget);

                    var delta = targetPos.Value - pos.Value;
                    var dist = math.length(delta);

                    // Not attacking
                    if (dist > attackDistance)
                    {
                        velocity.Value += delta * (chaseForce * deltaTime / dist);
                        smoothing.SmoothPosition = math.lerp(smoothing.SmoothPosition, pos.Value, deltaTime * rotationStiffness);
                    }

                    // Attacking
                    else
                    {
                        smoothing.SmoothPosition = pos.Value;

                        velocity.Value += delta * (attackForce * deltaTime / dist);

                        if (dist < hitDistance)
                        {
                            // Hit on enemy
                            ecb.AddComponent<DespawnTimer>(entityInQueryIndex, target.EnemyTarget);
                            ecb.AddComponent(entityInQueryIndex, target.EnemyTarget, new DespawnTimer { Time = 0.2f });
                            ecb.AddComponent<Gravity>(entityInQueryIndex, target.EnemyTarget);

                            ecb.SetComponent(entityInQueryIndex, entity, new Target { EnemyTarget = Entity.Null });
                        }
                    }
                }
                else if (target.ResourceTarget != Entity.Null)
                {
                    if (HasComponent<Carried>(target.ResourceTarget))
                    {
                        var homeField = HasComponent<TeamOne>(entity) ? teamOneFields[0] : teamTwoFields[0];
                        var homePos = homeField.Bounds.Center;
                        homePos.y = homeField.Bounds.Center.y - (homeField.Bounds.Extents.y * .9f);

                        var delta = homePos - pos.Value;
                        var dist = math.length(delta);
                        velocity.Value += delta * (carryForce * deltaTime / dist);

                        if (dist < 1f)
                        {
                            ecb.RemoveComponent<Carried>(entityInQueryIndex, target.ResourceTarget);

                            target.ResourceTarget = Entity.Null;
                        }
                    }
                    else
                    {
                        targetPos = GetComponent<Translation>(target.ResourceTarget);

                        var delta = targetPos.Value - pos.Value;
                        var dist = math.length(delta);

                        if (dist > grabDistance)
                        {
                            velocity.Value += delta * (chaseForce * deltaTime / dist);
                        }
                        else
                        {
                            ecb.AddComponent(entityInQueryIndex, target.ResourceTarget, new Carried() { Value = entity });
                        }
                    }
                }

                smoothing.SmoothDirection = smoothing.SmoothPosition - oldSmoothPos;
            }).ScheduleParallel();
    }
}
