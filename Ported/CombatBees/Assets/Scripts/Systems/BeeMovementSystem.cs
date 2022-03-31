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
    private const float FLIGHT_JITTER = 200.0f;
    private const float DAMPING = 0.1f;

    private const float TEAM_ATTRACTION = 5.0f;
    private const float TEAM_REPULSION = 4.0f;

    private const float CHASE_FORCE = 50.0f;
    private const float CARRY_FORCE = 25.0f;
    private const float GRAB_DISTANCE = 0.5f;

    private const float AGGRESSION = 0.5f;
    private const float ATTACK_DISTANCE = 4.0f;
    private const float ATTACK_FORCE = 500.0f;
    private const float HIT_DISTANCE = 0.5f;

    private Random _random;

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
                        delta = GetComponent<PositionComponent>(repulsiveFriend).Value;
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
                                        // TODO: Spawn Particle.

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
                            else if (resourceHeldByBee.Value == entity) // Holding the resource, carry it home.
                            {
                                // TODO:
                                //Vector3 targetPos = new Vector3(-Field.size.x * .45f + Field.size.x * .9f * bee.team, 0f, bee.position.z);
                                //delta = targetPos - bee.position;
                                //dist = Mathf.Sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
                                //bee.velocity += (targetPos - bee.position) * (carryForce * deltaTime / dist);
                                //if (dist < 1f)
                                //{
                                //    resource.holder = null;
                                //    bee.resourceTarget = null;
                                //}
                                //else
                                //{
                                //    bee.isHoldingResource = true;
                                //}
                            }
                            else if (false) // TODO: If target resource is held by enemy bee.
                            {
                                // TODO: Set our target to the enemy bee.
                            }
                            else // Target is held by friendly bee.
                            {
                                target.Value = default;
                            }
                        }
                    }
                    else // This bee is dead.
                    {
                        // TODO:
                        //if (Random.value < (bee.deathTimer - .5f) * .5f)
                        //{
                        //    ParticleManager.SpawnParticle(bee.position, ParticleType.Blood, Vector3.zero);
                        //}

                        //bee.velocity.y += Field.gravity * deltaTime;
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

                    // TODO: Keep bee in bounds.
                    //if (System.Math.Abs(bee.position.x) > Field.size.x * .5f)
                    //{
                    //    bee.position.x = (Field.size.x * .5f) * Mathf.Sign(bee.position.x);
                    //    bee.velocity.x *= -.5f;
                    //    bee.velocity.y *= .8f;
                    //    bee.velocity.z *= .8f;
                    //}
                    //if (System.Math.Abs(bee.position.z) > Field.size.z * .5f)
                    //{
                    //    bee.position.z = (Field.size.z * .5f) * Mathf.Sign(bee.position.z);
                    //    bee.velocity.z *= -.5f;
                    //    bee.velocity.x *= .8f;
                    //    bee.velocity.y *= .8f;
                    //}
                    //float resourceModifier = 0f;
                    //if (bee.isHoldingResource)
                    //{
                    //    resourceModifier = ResourceManager.instance.resourceSize;
                    //}
                    //if (System.Math.Abs(bee.position.y) > Field.size.y * .5f - resourceModifier)
                    //{
                    //    bee.position.y = (Field.size.y * .5f - resourceModifier) * Mathf.Sign(bee.position.y);
                    //    bee.velocity.y *= -.5f;
                    //    bee.velocity.z *= .8f;
                    //    bee.velocity.x *= .8f;
                    //}

                    // TODO: The example does most stuff in fixed update, but some in update, like the stretching, part of the rotation, and scale and coloring of dead bees.

                    // TODO: Scale (from Update)
                    //float size = bees[i].size;
                    //Vector3 scale = new Vector3(size, size, size);
                    //if (bees[i].dead == false)
                    //{
                    //    float stretch = Mathf.Max(1f, bees[i].velocity.magnitude * speedStretch);
                    //    scale.z *= stretch;
                    //    scale.x /= (stretch - 1f) / 5f + 1f;
                    //    scale.y /= (stretch - 1f) / 5f + 1f;
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