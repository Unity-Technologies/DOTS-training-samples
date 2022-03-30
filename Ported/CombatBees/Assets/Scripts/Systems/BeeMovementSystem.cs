using System.Collections.Generic;
using System.Resources;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public partial class BeeMovementSystem : SystemBase
{
    private const float MIN_BEE_SIZE = 0.25f;
    private const float MAX_BEE_SIZE = 0.50f;

    private const float SPEED_STRETCH = 0.20f;
    private const float ROTATION_STIFFNESS = 5.0f;
    private const float FLIGHT_JITTER = 200.0f;
    private const float DAMPING = 0.10f;

    private const float TEAM_ATTRACTION = 5.0f;
    private const float TEAM_REPULSION = 4.0f;

    private const float CHASE_FORCE = 50.0f;
    private const float CARRY_FORCE = 25.0f;
    private const float GRAB_DISTANCE = 0.5f;

    private const float AGGRESSION = 0.50f;
    private const float ATTACK_DISTANCE = 4.0f;
    private const float ATTACK_FORCE = 500.0f;
    private const float HIT_DISTANCE = 0.50f;

    private Random _random;

    protected override void OnCreate()
    {
        base.OnCreate();
        _random = new Random(1); // TODO: Seeing with 1 for debug, feed it time when ready. 
    }

    // The team decided the goal here is to exactly replicate the example movement behaviour and targeting logic.
    protected override void OnUpdate()
    {
        var deltaTime = Time.fixedDeltaTime;

        var yellowBeeEntities = GetEntityQuery(typeof(TeamYellowTagComponent)).ToEntityArray(Allocator.Temp);
        var blueBeeEntities = GetEntityQuery(typeof(TeamBlueTagComponent)).ToEntityArray(Allocator.Temp);
        var resourceEntities = GetEntityQuery(typeof(ResourceTagComponent)).ToEntityArray(Allocator.Temp);

        Entities
            .WithAll<TeamYellowTagComponent>()
            .ForEach((Entity entity, ref Translation translation, ref Rotation rotation, ref NonUniformScale scale,
                ref VelocityComponent velocity, ref HeldHoldingComponent holding, ref TargetComponent target) =>
                {
                    // A bee is dead, but not destroyed, if it targets itself.
                    if (entity != target.Value)
                    {
                        // Flag for the rotation behaviour.
                        var attacking = false;

                        // TODO: Flight Jitter

                        // TODO: Attract / Repulse

                        // var attractiveFriend = yellowTeamBeeEntities[_random.NextInt(yellowTeamBeeEntities.Length)];
                        // var repulsiveFriend = yellowTeamBeeEntities[_random.NextInt(yellowTeamBeeEntities.Length)];
                        // var enemyBee = blueTeamBeeEntities[_random.NextInt(blueTeamBeeEntities.Length)];
                        // var targetPosition = GetComponent<Translation>(enemyBee).Value;

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
                        else if (HasComponent<TargetComponent>(target.Value)) // Our target is a bee, because it has the target component.
                        {
                            var targetsTarget = GetComponent<TargetComponent>(target.Value).Value;
                            
                            // A bee is dead, but not destroyed, if it targets itself.
                            if (targetsTarget == target.Value)
                            {
                                target.Value = default;
                            }
                            else // Target bee still alive, chase or attack it.
                            {
                                var targetPosition = GetComponent<Translation>(target.Value);
                                var toVector = targetPosition.Value - translation.Value; // TODO: Once we add the attract / repulse, this can be a reused variable in the outer scope.
                                var sqrDist = toVector.x * toVector.x + toVector.y * toVector.y + toVector.z;

                                if (sqrDist > ATTACK_DISTANCE * ATTACK_DISTANCE)
                                {
                                    velocity.Value += toVector * (CHASE_FORCE * deltaTime / math.sqrt(sqrDist));
                                }
                                else
                                {
                                    attacking = true;

                                    velocity.Value += toVector * (ATTACK_FORCE * deltaTime / math.sqrt(sqrDist));

                                    if (sqrDist < HIT_DISTANCE * HIT_DISTANCE)
                                    {
                                        // TODO: Spawn Particle.

                                        // Bee is dead, but not destroyed, if it targets itself.
                                        SetComponent(targetsTarget, target);

                                        // TODO: Half the enemy bee's velocity.

                                        target.Value = default;
                                    }
                                }
                            }
                        }
                        else // The example explicitly checks this but if we've made it here then we have a resource target.
                        {
                            var resourceHolder = GetComponent<HeldHoldingComponent>(target.Value);

                            // Resources need to have the concept of "dead" for the period of time where they fall after
                            // being delivered or the carrier was killed, if our target is dead clear is.
                            if (resourceHolder.Value == target.Value)
                            {
                                target.Value = default;
                            }
                            else if (resourceHolder.Value == default) // If the resource has not been told it's held by a bee.
                            {
                                if (false) // TODO: Determine if the resource is in a stack AND is no longer the top of the stack.
                                {
                                    // This is for the case where the user has spawned new resources on top of the target resource before we got there.

                                    target.Value = default;
                                }
                                else // Move to the resource and "grab" it when close enough.
                                {
                                    var toVector = GetComponent<Translation>(target.Value).Value - translation.Value;
                                    var sqrDist = toVector.x * toVector.x + toVector.y * toVector.y + toVector.z;
                                    if (sqrDist > GRAB_DISTANCE * GRAB_DISTANCE)
                                    {
                                        velocity.Value += toVector * (CHASE_FORCE * deltaTime / math.sqrt(sqrDist));
                                    }
                                    else
                                    {
                                        resourceHolder.Value = entity;
                                        holding.Value = target.Value;
                                        
                                        // TODO: Remove resource from the grid stack, so the next resource in the stack can be targeted.
                                    }
                                }
                            }
                            else if (resourceHolder.Value == entity) // Holding the resource, carry it home.
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
                    else // Target bee is dead.
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

                    // TODO: Rotate bee.
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

                }).ScheduleParallel();

        Entities
            .WithAll<TeamBlueTagComponent>()
            .ForEach((Entity entity, ref Translation translation, ref Rotation rotation, ref NonUniformScale scale,
                ref VelocityComponent velocity, ref TargetComponent target) =>
                {
                    var isBeeDead = entity.Equals(target.Value);
                    if (isBeeDead == false)
                    {

                    }
                }).ScheduleParallel();
    }

    private static void ExampleMovementBehaviour()
    {

    }
}