using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Mathematics;

class BeeSimulationSystem: SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        const float aggressivity = 0.5f;
        const float speed = 1.0f;
        const float beeSize = 0.01f;
        const float targetSize = 0.01f;
        float collR2 = beeSize + targetSize;
        collR2 *= collR2;
        Random rng = new Random(123);

        EntityCommandBuffer ecb = new EntityCommandBuffer( Allocator.TempJob );
        var parallelECB = ecb.AsParallelWriter();
        var teamAQuery = GetEntityQuery(ComponentType.ReadOnly<TeamA>());
        NativeArray<Entity> teamABees = teamAQuery.ToEntityArray( Allocator.TempJob );
        var teamBQuery = GetEntityQuery(ComponentType.ReadOnly<TeamB>());
        NativeArray<Entity> teamBBees = teamBQuery.ToEntityArray( Allocator.TempJob );
        var resourceQuery = GetEntityQuery(ComponentType.ReadOnly<Resource>());
        NativeArray<Entity> resources = resourceQuery.ToEntityArray( Allocator.TempJob );
        Entities
            .WithReadOnly(teamABees)
            .WithReadOnly(teamBBees)
            .WithReadOnly(resources)
            .ForEach((Entity entity, ref Bee bee, ref NewTranslation pos) =>
            {
                // set target if not set
                if(bee.Target == Entity.Null)
                {
                    bool fetchResource = resources.Length > 0 && rng.NextFloat() < aggressivity;
                    if(!fetchResource)
                    {
                        if(HasComponent<TeamA>(entity))
                        {
                            if(teamBBees.Length>0)
                            {
                                int beeIdx = rng.NextInt(teamBBees.Length);
                                bee.Target = teamBBees[beeIdx];
                            }
                            else
                                fetchResource = true;
                        }
                        else
                        {
                            if(teamBBees.Length>0)
                            {
                                int beeIdx = rng.NextInt(teamABees.Length);
                                bee.Target = teamABees[beeIdx];
                            }
                            else
                                fetchResource = true;
                        }
                        bee.State = BeeState.ChasingEnemy;
                    }
                    if(fetchResource)
                    {
                        int resourceIdx = rng.NextInt(resources.Length);
                        bee.Target = resources[resourceIdx];
                        bee.State = BeeState.GettingResource;
                    }
                }

                // move bee towards the target
                if(!HasComponent<Translation>(bee.Target))
                {
                    bee.Target = Entity.Null;
                    bee.State = BeeState.Idle;
                    return;
                }
                var targetPos = GetComponent<Translation>(bee.Target);
                float3 targetVec = targetPos.Value - pos.translation.Value;
                float3 dir = math.normalize(targetVec);
                pos.translation.Value += dir * speed * deltaTime;

                // check collision with target
                if(math.lengthsq(targetVec) < collR2)
                {
                    switch(bee.State)
                    {
/*                        case BeeState.Idle:
                        {
                        } break;
*/
                        case BeeState.GettingResource:
                        {
                            var resource = GetComponent<Resource>(bee.Target);
                            resource.CarryingBee = entity;
                            bee.resource = bee.Target;
                            bee.Target = Entity.Null; /*todo: set base entity*/
                            bee.State = BeeState.ReturningToBase;
                        } break;

                        case BeeState.ReturningToBase:
                        {
                            var resource = GetComponent<Resource>(bee.Target);
                            resource.CarryingBee = Entity.Null;
                            bee.resource = Entity.Null;
                            bee.Target = Entity.Null;
                            bee.State = BeeState.Idle;
                        } break;

                        case BeeState.ChasingEnemy:
                        {
                            parallelECB.DestroyEntity(0, bee.Target);
                            bee.Target = Entity.Null;
                            bee.State = BeeState.Idle;
                        } break;
                    }
                }
            }).ScheduleParallel();
        Dependency.Complete();
        ecb.Playback(EntityManager);
        ecb.Dispose();
        teamABees.Dispose();
        teamBBees.Dispose();
        resources.Dispose();
    }
}
