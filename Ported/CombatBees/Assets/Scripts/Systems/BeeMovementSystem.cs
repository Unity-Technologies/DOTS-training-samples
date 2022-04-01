using Components;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Mathf = UnityEngine.Mathf;
using Unity.Collections;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(ParticleSystemFixed))]
[UpdateAfter(typeof(Systems.TargetSystem))]
public partial class BeeMovementSystemFixed : SystemBase
{
    static readonly float flightJitter = 200f;
    static readonly float damping = 0.1f;
    static readonly float speedStretch = 0.2f;
    static readonly float teamAttraction = 5f;
    static readonly float teamRepulsion = 4f;

    static readonly float chaseForce = 50f;
    static readonly float carryForce = 25f;
    static readonly float attackDistance = 4f;
    static readonly float attackForce = 500f;
    static readonly float hitDistance = 0.5f;
    static readonly float grabDistance = 0.5f;

    EntityQuery[] teamTargets;
    EntityCommandBufferSystem beginFixedSimulationEntityCommandBufferSystem;
    EntityCommandBufferSystem endFixedSimulationEntityCommandBufferSystem;

    protected override void OnCreate()
    {
        teamTargets = new EntityQuery[2]
        {
            EntityManager.CreateEntityQuery(ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<TeamShared>()),
            EntityManager.CreateEntityQuery(ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<TeamShared>())
        };
        teamTargets[0].SetSharedComponentFilter(new TeamShared { TeamId = 0 });
        teamTargets[1].SetSharedComponentFilter(new TeamShared { TeamId = 1 });

        beginFixedSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<BeginFixedStepSimulationEntityCommandBufferSystem>();
        endFixedSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var particles = GetSingleton<ParticleSettings>();
        var deltaTime = Time.DeltaTime;

        var team0 = teamTargets[0].ToComponentDataArray<Translation>(Allocator.TempJob);
        var team1 = teamTargets[1].ToComponentDataArray<Translation>(Allocator.TempJob);

        var beginFrameEcb = beginFixedSimulationEntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
        var endFrameEcb = endFixedSimulationEntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        var gsv = GlobalSystemVersion;

        // Basic movement for alive bees
        Dependency = Entities
            .WithNone<BeeLifetime>()
            .WithReadOnly(team0)
            .WithDisposeOnCompletion(team0)
            .WithReadOnly(team1)
            .WithDisposeOnCompletion(team1)
            .ForEach((Entity entity, ref Translation translation, ref Velocity velocity, ref AttractionRepulsion attractionRepulsion, in TargetType targetType, in Team team) =>
            {
                var random = Random.CreateFromIndex(gsv ^ (uint)entity.Index);

                if (team.TeamId == 0)
                {
                    if (team0.Length > 0)
                    {
                        attractionRepulsion.AttractionPos = team0[random.NextInt(team0.Length)].Value;
                        attractionRepulsion.RepulsionPos = team0[random.NextInt(team0.Length)].Value;
                    }
                }
                else if (team1.Length > 0)
                {
                    attractionRepulsion.AttractionPos = team1[random.NextInt(team1.Length)].Value;
                    attractionRepulsion.RepulsionPos = team1[random.NextInt(team1.Length)].Value;
                }

                var position = translation.Value;
                UpdateJitterAndTeamVelocity(ref random, ref velocity.Value, in position, in attractionRepulsion, deltaTime);

                translation.Value += velocity.Value * deltaTime;
                UpdateBorders(ref velocity.Value, ref translation.Value, targetType.Value == TargetType.Type.Goal);

            }).ScheduleParallel(Dependency);


        // Movement for dead bees
        Dependency = Entities
            .WithAll<BeeLifetime>()
            .ForEach((Entity entity, ref Translation translation, ref Velocity velocity) =>
            {
                translation.Value += velocity.Value * deltaTime;
                UpdateBorders(ref velocity.Value, ref translation.Value, false);

            }).ScheduleParallel(Dependency);


        // Don't run target tasks for dead bees
        Dependency = Entities
            .WithNone<BeeLifetime>()
            .ForEach((Entity entity,
                int entityInQueryIndex,
                ref BeeMovement bee,
                ref Velocity velocityComp,
                ref TargetType targetType,
                ref TargetEntity targetEntity,
                in Translation translation,
                in Team team) =>
            {
                var random = Random.CreateFromIndex(gsv ^ (uint)entity.Index);

                var velocity = velocityComp.Value;
                var position = translation.Value;

                bee.IsAttacking = 0;
          
                if (targetType.Value == TargetType.Type.Enemy)
                {
                    if (HasComponent<BeeLifetime>(targetEntity.Value))
                    {
                        targetType.Value = TargetType.Type.None;
                    }
                    else
                    {
                        var delta = targetEntity.Position - position;
                        float sqrDist = delta.x * delta.x + delta.y * delta.y + delta.z * delta.z;
                        if (sqrDist > attackDistance * attackDistance)
                        {
                            velocity += delta * (chaseForce * deltaTime / Mathf.Sqrt(sqrDist));
                        }
                        else
                        {
                            velocity += delta * (attackForce * deltaTime / Mathf.Sqrt(sqrDist));
                            bee.IsAttacking = 1;
                            if (sqrDist < hitDistance * hitDistance)
                            {
                                ParticleSystem.SpawnParticle(beginFrameEcb, entityInQueryIndex, particles.Particle, ref random,
                                    targetEntity.Position, ParticleComponent.ParticleType.Blood, velocity * .35f, 2f, 6);

                                endFrameEcb.AddComponent<BeeLifetime>(entityInQueryIndex, targetEntity.Value, new BeeLifetime
                                {
                                    Value = 1f,
                                    NewlyDead = 1
                                });
                                targetType.Value = TargetType.Type.None;
                            }
                        }
                    }
                }
                else if (targetType.Value == TargetType.Type.Resource)
                {
                    if ((HasComponent<ResourceOwner>(targetEntity.Value)))
                    {
                        var otherOwner = GetComponent<ResourceOwner>(targetEntity.Value).Owner;
                        var otherTeam = GetComponent<Team>(otherOwner).TeamId;
                        if (otherTeam != team.TeamId)
                        {
                            // Attack whoever stole it
                            targetEntity.Value = otherOwner;
                            targetType.Value = TargetType.Type.Enemy;
                        }
                    }
                    
                    if (targetType.Value == TargetType.Type.Resource)
                    {
                        if (!HasComponent<Components.Resource>(targetEntity.Value)
                            || (HasComponent<ResourceOwner>(targetEntity.Value)
                            && GetComponent<ResourceOwner>(targetEntity.Value).Owner != entity))
                        {
                            targetType.Value = TargetType.Type.None;
                        }
                        else
                        {
                            var delta = targetEntity.Position - position;
                            float sqrDist = delta.x * delta.x + delta.y * delta.y + delta.z * delta.z;
                            if (sqrDist > grabDistance * grabDistance)
                            {
                                velocity += delta * (carryForce * deltaTime / Mathf.Sqrt(sqrDist));
                            }
                            else
                            {
                                endFrameEcb.AddComponent<ResourceOwner>(entityInQueryIndex, targetEntity.Value,
                                    new ResourceOwner() { Owner = entity });
                                endFrameEcb.SetComponent<Components.Resource>(entityInQueryIndex, targetEntity.Value,
                                    new Components.Resource() { OwnerPosition = position - new float3(0, PlayField.resourceHeight, 0) });
                                targetType.Value = TargetType.Type.Goal;
                            }
                        }
                        
                    }
                }
                else if (targetType.Value == TargetType.Type.Goal)
                {
                    if (!HasComponent<Components.Resource>(targetEntity.Value))
                    {
                        targetType.Value = TargetType.Type.None;
                    }
                    else
                    {
                        var delta = new float3(-PlayField.size.x * .45f + PlayField.size.x * .9f * team.TeamId, 0f,
                            position.z) - position;
                        float sqrDist = delta.x * delta.x + delta.y * delta.y + delta.z * delta.z;
                        if (sqrDist > grabDistance * grabDistance)
                        {
                            velocity += delta * (chaseForce * deltaTime / Mathf.Sqrt(sqrDist));
                        }
                        else
                        {
                            targetType.Value = TargetType.Type.None;

                            endFrameEcb.RemoveComponent<ResourceOwner>(entityInQueryIndex, targetEntity.Value);
                            endFrameEcb.AddComponent<KinematicBody>(entityInQueryIndex, targetEntity.Value,
                                new KinematicBody() { landPosition = -PlayField.size.y * 0.5f });
                        }
                    }
                }

                velocityComp.Value = velocity;

            }).ScheduleParallel(Dependency);


        Dependency = Entities
           .ForEach((ref MovementSmoothing smoothing, in Translation translation, in BeeMovement movement) =>
           {
               // only used for smooth rotation:
               float3 oldSmoothPos = smoothing.SmoothPosition;
               if (movement.IsAttacking == 0)
               {
                   smoothing.SmoothPosition = math.lerp(smoothing.SmoothPosition, translation.Value, deltaTime * /*rotationStiffness*/5);
               }
               else
               {
                   smoothing.SmoothPosition = translation.Value;
               }
               smoothing.SmoothDirection = smoothing.SmoothPosition - oldSmoothPos;

           }).ScheduleParallel(Dependency);

        // Dead bee cleanup
        Dependency = Entities
            .ForEach((Entity entity,
            int entityInQueryIndex,
             ref BeeLifetime life,
             ref Velocity velocity,
            ref Translation translation) =>
            {
                if (life.NewlyDead == 1)
                {
                    velocity.Value *= .5f;
                    life.NewlyDead = 0;
                }

                var random = Random.CreateFromIndex(gsv ^ (uint)entity.Index);

                if (random.NextFloat(1f) < (life.Value - .5f) * .5f)
                {
                    ParticleSystem.SpawnParticle(beginFrameEcb, entityInQueryIndex, particles.Particle, ref random, translation.Value, ParticleComponent.ParticleType.Blood, float3.zero);
                }

                velocity.Value.y += PlayField.gravity * deltaTime;
                translation.Value += velocity.Value * deltaTime;

                life.Value -= deltaTime / 10f;
                if (life.Value < 0f)
                {
                    beginFrameEcb.DestroyEntity(entityInQueryIndex, entity);
                }
            }).ScheduleParallel(Dependency);

        beginFixedSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        endFixedSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }

    private static void UpdateJitterAndTeamVelocity(ref Random random, ref float3 velocity, in float3 position,
        in AttractionRepulsion attraction, float deltaTime)
    {
        velocity += random.NextFloat3Direction() * (flightJitter * deltaTime);
        velocity *= 1f - damping;

        var delta = attraction.AttractionPos - position;
        float dist = Mathf.Sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
        if (dist > 0f)
        {
            velocity += delta * (teamAttraction * deltaTime / dist);
        }

        delta = attraction.RepulsionPos - position;
        dist = Mathf.Sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
        if (dist > 0f)
        {
            velocity -= delta * (teamRepulsion * deltaTime / dist);
        }
    }

    private static void UpdateBorders(ref float3 velocity, ref float3 position, bool isHoldingResource)
    {
        if (Mathf.Abs(position.x) > PlayField.size.x * .5f)
        {
            position.x = PlayField.size.x * .5f * Mathf.Sign(position.x);
            velocity.x *= -0.5f;
            velocity.y *= .8f;
            velocity.z *= .8f;
        }

        if (Mathf.Abs(position.z) > PlayField.size.z * .5f)
        {
            position.z = PlayField.size.z * .5f * Mathf.Sign(position.z);
            velocity.z *= -0.5f;
            velocity.x *= .8f;
            velocity.y *= .8f;
        }
        float resMod = isHoldingResource ? PlayField.resourceHeight : 0;
        if (Mathf.Abs(position.y) > PlayField.size.y * .5f - resMod)
        {
            position.y = (PlayField.size.y * .5f - resMod) * Mathf.Sign(position.y);
            velocity.y *= -0.5f;
            velocity.z *= .8f;
            velocity.x *= .8f;
        }
    }
}

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(ParticleSystemFixed))]
[UpdateAfter(typeof(Systems.TargetSystem))]
public partial class BeeMovementSystem : SystemBase
{
    static readonly float speedStretch = 0.2f;

    protected override void OnUpdate()
    {
        // Alive bees
        Dependency = Entities
            .WithNone<BeeLifetime>()
            .ForEach((ref NonUniformScale scale, ref Rotation rotation, in Velocity velocity, in MovementSmoothing smoothing) =>
            {
                var size = smoothing.Size;
                var scl = new float3(size, size, size);
                float stretch = Mathf.Max(1f, math.length(velocity.Value) * speedStretch);
                scl.z *= stretch;
                scl.x /= (stretch - 1f) / 5f + 1f;
                scl.y /= (stretch - 1f) / 5f + 1f;
                scale.Value = scl;

                if (!smoothing.SmoothDirection.Equals(float3.zero))
                {
                    rotation.Value = quaternion.LookRotation(smoothing.SmoothDirection, new float3(0, 1, 0));
                }

            }).ScheduleParallel(Dependency);

        // Dead bees
        Dependency = Entities
            .ForEach((ref NonUniformScale scale, ref Rotation rotation, in BeeLifetime life, in MovementSmoothing smoothing) =>
            {
                var size = smoothing.Size;
                var scl = new float3(size, size, size);
                scale.Value = scl * Mathf.Sqrt(life.Value);                

                if (!smoothing.SmoothDirection.Equals(float3.zero))
                {
                    rotation.Value = quaternion.LookRotation(smoothing.SmoothDirection, new float3(0, 1, 0));
                }

            }).ScheduleParallel(Dependency);
    }
}