using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

[UpdateAfter(typeof(ResourceSpawnerSystem))]
public partial class BeeMovementSystem : SystemBase
{
    private const float SPEED_STRETCH = 0.2f;
    private const float ROTATION_STIFFNESS = 5.0f;
    private const float FLIGHT_JITTER = 1000.0f;
    private const float DAMPING = 0.1f;

    private const float TEAM_ATTRACTION = 20.0f;
    private const float TEAM_REPULSION = 16.0f;

    private const float CHASE_FORCE = 200.0f;

    private const float CARRY_FORCE = 100.0f;
    private const float RESOURCE_SIZE = 0.75f;
    private const float GRAB_DISTANCE = 0.5f;

    private const float AGGRESSION = 0.5f;
    private const float ATTACK_FORCE = 2000.0f;
    private const float ATTACK_DISTANCE = 4.0f;
    private const float HIT_DISTANCE = 0.5f;

    private Random _random; // TODO: Need to ask about using this in parallel. 

    protected override void OnCreate()
    {
        base.OnCreate();
        _random = new Random(1); // TODO: Seeding with 1 for debug, feed it time when ready. 
    }

    // The team decided the goal here is to exactly replicate the example movement behaviour and targeting logic.
    protected override void OnUpdate()
    {
        // The example actually updates MOST of this in FixedUpdate. Could set this up to run at the same rate?
        var deltaTime = Time.DeltaTime;

        // TODO: Ask about disposing these.
        var yellowBeeEntities = GetEntityQuery(typeof(TeamYellowTagComponent)).ToEntityArray(Allocator.TempJob);
        var blueBeeEntities = GetEntityQuery(typeof(TeamBlueTagComponent)).ToEntityArray(Allocator.TempJob);
        var resourceEntities = GetEntityQuery(typeof(ResourceTagComponent)).ToEntityArray(Allocator.TempJob);
        
        // Store all the positions in a component for later use.
        Entities
            .WithAll<PositionComponent>()
            .ForEach((ref PositionComponent position, in Translation translation) =>
            {
                position.Value = translation.Value;
            }).ScheduleParallel();
        
        // Workaround because you can't write to other entities from within a foreach.
        var teamBlueTargetData = GetComponentDataFromEntity<TeamBlueTargetComponent>();
        var heldByBeeData = GetComponentDataFromEntity<HeldByBeeComponent>();

        Entities
            .WithNativeDisableContainerSafetyRestriction(teamBlueTargetData)
            .WithNativeDisableContainerSafetyRestriction(heldByBeeData)
            .WithoutBurst()
            .WithAll<TeamYellowTagComponent>()
            .ForEach((Entity entity, ref Translation translation, ref Rotation rotation, ref NonUniformScale scale,
                ref VelocityComponent velocity, ref HeldResourceComponent heldResource, ref TeamYellowTargetComponent target, in PositionComponent position) =>
                {
                    // Is bee flagged as dead?
                    if (entity != target.Value)
                    {
                        // Flag for the rotation behaviour.
                        var attacking = false;

                        velocity.Value += _random.NextFloat3Direction() * (FLIGHT_JITTER * deltaTime);
                        velocity.Value *= (1.0f - DAMPING);

                        var attractiveFriend = yellowBeeEntities[_random.NextInt(yellowBeeEntities.Length)];
                        var delta = GetComponent<PositionComponent>(attractiveFriend).Value - translation.Value;
                        var dist = math.sqrt((delta.x * delta.x) + (delta.y * delta.y) + (delta.z * delta.z));
                        if (dist > 0.0f)
                        {
                            velocity.Value += delta * (TEAM_ATTRACTION * deltaTime / dist);
                        }

                        var repulsiveFriend = yellowBeeEntities[_random.NextInt(yellowBeeEntities.Length)];
                        delta = GetComponent<PositionComponent>(repulsiveFriend).Value - translation.Value;
                        dist = math.sqrt((delta.x * delta.x) + (delta.y * delta.y) + (delta.z * delta.z));
                        if (dist > 0.0f)
                        {
                            velocity.Value -= delta * (TEAM_REPULSION * deltaTime / dist);
                        }

                        // No target entity, find either an enemy bee or a resource, depending on aggression. 
                        if (target.Value == default)
                        {
                            if (_random.NextFloat(1.0f) < AGGRESSION)
                            {
                                if (blueBeeEntities.Length > 0)
                                {
                                    target.Value = blueBeeEntities[_random.NextInt(blueBeeEntities.Length)];
                                }
                            }
                            else
                            {
                                if (resourceEntities.Length > 0)
                                {
                                    target.Value = resourceEntities[_random.NextInt(resourceEntities.Length)];
                                }
                            }
                        }
                        else if (HasComponent<TeamBlueTagComponent>(target.Value)) // Our target is an enemy bee.
                        {
                            var targetsTarget = GetComponent<TeamBlueTargetComponent>(target.Value);

                            // A bee is dead, but not destroyed, if it targets itself.
                            if (targetsTarget.Value == target.Value)
                            {
                                target.Value = default;
                            }
                            else // Target bee still alive, chase or attack it.
                            {
                                var targetPosition = GetComponent<PositionComponent>(target.Value);
                                delta = targetPosition.Value - translation.Value;
                                var sqrDist = (delta.x * delta.x) + (delta.y * delta.y) + (delta.z * delta.z);

                                if (sqrDist > ATTACK_DISTANCE * ATTACK_DISTANCE)
                                {
                                    velocity.Value += delta * (CHASE_FORCE * deltaTime / math.sqrt(sqrDist));
                                }
                                else
                                {
                                    attacking = true;

                                    velocity.Value += delta * (ATTACK_FORCE * deltaTime / math.sqrt(sqrDist));

                                    if (sqrDist < HIT_DISTANCE * HIT_DISTANCE)
                                    {
                                        ParticleManager.SpawnParticle(targetPosition.Value, ParticleManager.ParticleType.Blood, velocity.Value * .35f, 2f, 6);

                                        // Bee is dead, but not destroyed, if it targets itself.
                                        targetsTarget.Value = target.Value;
                                        teamBlueTargetData[target.Value] = targetsTarget;
                                        //SetComponent(target.Value, targetsTarget);

                                        // TODO: Half the enemy bee's velocity.

                                        target.Value = default;
                                    }
                                }
                            }
                        }
                        else // The example explicitly checks this but if we've made it here then we have a resource target.
                        {
                            var resourceHeldByBee = GetComponent<HeldByBeeComponent>(target.Value);

                            // Resources need to have the concept of "dead" for the period of time where they fall after
                            // being delivered or the carrier was killed, if our target is dead(holding itself) clear is.
                            if (resourceHeldByBee.Value == target.Value)
                            {
                                target.Value = default;
                            }
                            else if (resourceHeldByBee.Value == default) // If the resource has not been told it's held by a bee.
                            {
                                if (false) // TODO: Determine if the resource is in a stack AND is no longer the top of the stack.
                                {
                                    // This is for the case where the user has spawned new resources on top of the target resource before we got there.

                                    target.Value = default;
                                }
                                else // Move to the resource and "grab" it when close enough.
                                {
                                    delta = GetComponent<PositionComponent>(target.Value).Value - translation.Value;
                                    var sqrDist = (delta.x * delta.x) + (delta.y * delta.y) + (delta.z * delta.z);
                                    if (sqrDist > GRAB_DISTANCE * GRAB_DISTANCE)
                                    {
                                        velocity.Value += delta * (CHASE_FORCE * deltaTime / math.sqrt(sqrDist));
                                    }
                                    else
                                    {
                                        resourceHeldByBee.Value = entity;
                                        heldByBeeData[target.Value] = resourceHeldByBee;
                                        // SetComponent(target.Value, resourceHeldByBee);

                                        heldResource.Value = target.Value;

                                        // TODO: Remove resource from the grid stack, so the next resource in the stack can be targeted.
                                    }
                                }
                            }
                            else if (resourceHeldByBee.Value == entity)
                            {
                                // Holding the resource, carry it home.
                                var targetPos = new float3(-Field.size.x * .45f + Field.size.x * .9f, 0f, translation.Value.z);
                                delta = targetPos - translation.Value;
                                dist = math.sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
                                velocity.Value += (targetPos - translation.Value) * (CARRY_FORCE * deltaTime / dist);
                                if (dist < 1f)
                                {
                                    resourceHeldByBee.Value = default;
                                    target.Value = default;
                                }
                            }
                            else if (HasComponent<TeamBlueTargetComponent>(resourceHeldByBee.Value)) // TODO: If target resource is held by enemy bee.
                            {
                                target.Value = resourceHeldByBee.Value;
                            }
                            else // Target is held by friendly bee.
                            {
                                target.Value = default;
                            }
                        }
                    }
                    else // This bee is dead.
                    {
                        // TODO: Add in the death timer and death stuff.
                        //if (_random < (bee.deathTimer - .5f) * .5f)
                        //{
                            ParticleManager.SpawnParticle(translation.Value, ParticleManager.ParticleType.Blood, float3.zero);
                        //}

                        velocity.Value.y += Field.gravity * deltaTime;
                        //bee.deathTimer -= deltaTime / 10f;
                        //if (bee.deathTimer < 0f)
                        //{
                        //    DeleteBee(bee);
                        //}

                        // TODO: Color / Scale for dead bees (from update)
                        //if (bees[i].dead)
                        //{
                        //    color *= .75f;
                        //    scale *= Mathf.Sqrt(bees[i].deathTimer);
                        //}
                    }

                    // Move this bee!!
                    translation.Value += deltaTime * velocity.Value;

                    // Keep bee in bounds.
                    if (math.abs(translation.Value.x) > Field.size.x * .5f)
                    {
                        translation.Value.x = (Field.size.x * .5f) * math.sign(translation.Value.x);
                        velocity.Value.x *= -.5f;
                        velocity.Value.y *= .8f;
                        velocity.Value.z *= .8f;
                    }
                    if (math.abs(translation.Value.z) > Field.size.z * .5f)
                    {
                        translation.Value.z = (Field.size.z * .5f) * math.sign(translation.Value.z);
                        velocity.Value.z *= -.5f;
                        velocity.Value.x *= .8f;
                        velocity.Value.y *= .8f;
                    }
                    float resourceModifier = 0f;
                    if (heldResource.Value != default)
                    {
                        resourceModifier = RESOURCE_SIZE;
                    }
                    if (math.abs(translation.Value.y) > Field.size.y * .5f - resourceModifier)
                    {
                        translation.Value.y = (Field.size.y * .5f - resourceModifier) * math.sign(translation.Value.y);
                        velocity.Value.y *= -.5f;
                        velocity.Value.z *= .8f;
                        velocity.Value.x *= .8f;
                    }

                    // TODO: The example does most stuff in fixed update, but some in update, like the stretching, part of the rotation, and scale and coloring of dead bees.

                    // Scale // TODO: Each bee needs to be given a random Size, and that needs to be tracked for use here.
                    //float size = bees[i].size;
                    //float baseSize = 0.5f;
                    //if (entity != target.Value)
                    //{
                    //    float stretch = math.max(1f, math.length(velocity.Value) * SPEED_STRETCH);
                    //    scale.Value.z = baseSize * stretch;
                    //    scale.Value.x = baseSize / (stretch - 1f) / 5f + 1f;
                    //    scale.Value.y = baseSize / (stretch - 1f) / 5f + 1f;
                    //}

                    // TODO: Rotation
                    //Vector3 oldSmoothPos = bee.smoothPosition;
                    //if (bee.isAttacking == false)
                    //{
                    //    bee.smoothPosition = Vector3.Lerp(bee.smoothPosition, bee.position, deltaTime * rotationStiffness);
                    //}
                    //else
                    //{
                    //    bee.smoothPosition = bee.position;
                    //}
                    //bee.smoothDirection = bee.smoothPosition - oldSmoothPos;

                    // TODO: Rotation (from update)
                    //Quaternion rotation = Quaternion.identity;
                    //if (bees[i].smoothDirection != Vector3.zero)
                    //{
                    //    rotation = Quaternion.LookRotation(bees[i].smoothDirection);
                    //}

                }).Run();

        // TODO: Foreach the other team of bees.

        // Attempting to separate the decision making from the movement by introducing a state component. // The concept of a "dead" but not destroyed bee is really making things difficult for me, so for this iteration I'm going to ignore that.
        //Entities
        //    .WithAll<TeamYellowTagComponent>()
        //    .ForEach((ref BeeStateComponent beeState, ref TargetComponent target, in PositionComponent position) =>
        //    {
        //        switch (beeState.Value)
        //        {
        //            case BeeState.NoTarget:
        //            {
        //                if (_random.NextFloat(1.0f) < AGGRESSION)
        //                {
        //                    if (blueBeeEntities.Length > 0)
        //                    {
        //                        target.Value = blueBeeEntities[_random.NextInt(blueBeeEntities.Length)];
        //                        beeState.Value = BeeState.ChaseEnemy;
        //                    }
        //                }
        //                else
        //                {
        //                    if (resourceEntities.Length > 0)
        //                    {
        //                        target.Value = resourceEntities[_random.NextInt(resourceEntities.Length)];
        //                        beeState.Value = BeeState.ChaseResource;
        //                    }
        //                }

        //                break;
        //            }

        //            case BeeState.ChaseEnemy:
        //            {
        //                if (HasComponent<TeamBlueTagComponent>(target.Value)) 
        //                {

        //                }
        //            }
        //                break;
        //            case BeeState.Attack:
        //                break;
        //            case BeeState.Grab:
        //                break;
        //            case BeeState.Carry:
        //                break;
        //            case BeeState.Dead:
        //                break;
        //            default:
        //                throw new ArgumentOutOfRangeException();
        //        }

        //        if (beeState.Value == BeeState.Dead)
        //        {
        //            // No target entity, find either an enemy bee or a resource, depending on aggression. 
        //            if (target.Value == default)
        //            {

        //            }
        //            else if (HasComponent<TeamBlueTagComponent>(target.Value)) // Target is enemy bee.
        //            {

        //            }
        //        }
        //    }).ScheduleParallel();

        yellowBeeEntities.Dispose();
        blueBeeEntities.Dispose();
        resourceEntities.Dispose();
    }

    

    private static void ExampleMovementBehaviour()
    {

    }
}