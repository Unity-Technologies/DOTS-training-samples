using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(BeeMovementSystem))]
public partial class ResourceMovementSystem : SystemBase
{
    private const float RESOURCE_SIZE = 0.75f;
    private const float SNAP_STIFFNESS = 2.0f;
    private const float CARRY_STIFFNESS = 15.0f;

    private const int BEES_PER_RESOURCE = 8;

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;

        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var parallelecb = ecb.AsParallelWriter();

        var cdfe = GetComponentDataFromEntity<VelocityComponent>(true);

        Entities
            .WithStructuralChanges()
            .WithNativeDisableContainerSafetyRestriction(cdfe)
            .WithoutBurst()
            .ForEach((Entity entity, int entityInQueryIndex, ref HeldByBeeComponent heldByBee, ref Translation translation, ref VelocityComponent velocity) =>
            {
                // TODO: It probably makes more sense to add/remove the held by bee component as it's pickedup/dropped by the bees.
                if (heldByBee.HoldingBee != default)
                {
                    var beeStateComponent = GetComponent<BeeStateComponent>(heldByBee.HoldingBee);
                    if (beeStateComponent.Value == BeeStateComponent.BeeState.Dead)
                    {
                        heldByBee.HoldingBee = default;
                    }
                    else
                    {
                        // TODO: Probably also makes sense for the carrying bee to update all this info into the HeldByComponent instead?
                        var beePosition = GetComponent<PositionComponent>(heldByBee.HoldingBee).Value;
                        var beeSize = GetComponent<BeeBaseSizeComponent>(heldByBee.HoldingBee).Value;
                        var beeVelocity = cdfe[heldByBee.HoldingBee].Value;

                        float3 targetPos = beePosition - math.up() * (RESOURCE_SIZE + beeSize) * .5f;
                        translation.Value = math.lerp(translation.Value, targetPos, CARRY_STIFFNESS * deltaTime);
                        velocity.Value = beeVelocity;
                    }
                }
                else
                {
                    // TODO: We probably should add a resource state component so we know if a resource is held, stacked, falling, etc.
                    // TODO: OR maybe base it off the existence of heldby in another loop. BUT it's the 11th hour here so I just want the functionality . 

                    // We dont really have a grid/stack(yet) system so I have to just base this on the Y where the spawner puts them.
                    var minY = (int)-Field.size.y / 2 + 1;
                    if ((int)translation.Value.y != minY)
                    {
                        velocity.Value.y += Field.gravity * deltaTime;
                        translation.Value += velocity.Value * deltaTime;

                        for (int j = 0; j < 3; j++)
                        {
                            if (System.Math.Abs(translation.Value[j]) > Field.size[j] * .5f)
                            {
                                translation.Value[j] = Field.size[j] * .5f * math.sign(translation.Value[j]);
                                velocity.Value[j] *= -.5f;
                                velocity.Value[(j + 1) % 3] *= .8f;
                                velocity.Value[(j + 2) % 3] *= .8f;
                            }
                        }

                        if (translation.Value.y < minY)
                        {
                            translation.Value.y = minY;

                            if (math.abs(translation.Value.x) > Field.size.x * .4f)
                            {
                                int team = 0;
                                if (translation.Value.x > 0f)
                                    team = 1;

                                //for (int j = 0; j < BEES_PER_RESOURCE; j++)
                                //{
                                //    // BeeManager.SpawnBee(resource.position, team);
                                //}

                                ParticleManager.SpawnParticle(translation.Value, ParticleManager.ParticleType.SpawnFlash, float3.zero, 6f, 5);

                                //EntityManager.DestroyEntity(entity);
                            }
                        }
                    }
                }

            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
