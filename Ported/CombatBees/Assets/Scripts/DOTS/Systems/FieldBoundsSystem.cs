using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public class FieldBoundsSystem : SystemBase
{
    EntityQuery m_MainFieldQuery;

    protected override void OnCreate()
    {
        m_MainFieldQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<FieldInfo>()
            },
            None = new[]
            {
                ComponentType.ReadOnly<TeamOne>(),
                ComponentType.ReadOnly<TeamTwo>()
            }
        });
    }

    protected override void OnUpdate()
    {
        var mainField = m_MainFieldQuery.ToComponentDataArrayAsync<FieldInfo>(Unity.Collections.Allocator.TempJob, out var mainFieldHandle);

        Dependency = JobHandle.CombineDependencies(Dependency, mainFieldHandle);

        Entities
            .WithAny<TeamOne, TeamTwo>()
            .WithDeallocateOnJobCompletion(mainField)
            .ForEach((ref Velocity velocity, ref Translation translation) =>
            {
                for (int j = 0; j < mainField.Length; ++j)
                {
                    var bound = mainField[j].Bounds;

                    if (math.abs(translation.Value.x) > bound.Size.x * .5f)
                    {
                        translation.Value.x = (bound.Size.x * .5f) * math.sign(translation.Value.x);
                        velocity.Value.x *= -.5f;
                        velocity.Value.y *= .8f;
                        velocity.Value.z *= .8f;
                    }

                    if (math.abs(translation.Value.z) > bound.Size.z * .5f)
                    {
                        translation.Value.z = (bound.Size.z * .5f) * math.sign(translation.Value.z);
                        velocity.Value.z *= -.5f;
                        velocity.Value.x *= .8f;
                        velocity.Value.y *= .8f;
                    }

                    // float resourceModifier = 0f;
                    // if (bee.isHoldingResource) {
                    //     resourceModifier = ResourceManager.instance.resourceSize;
                    // }

                    if (math.abs(translation.Value.y) > bound.Size.y * .5f /* - resourceModifier*/)
                    {
                        translation.Value.y = (bound.Size.y * .5f /* - resourceModifier*/) * math.sign(translation.Value.y);
                        velocity.Value.y *= -.5f;
                        velocity.Value.x *= .8f;
                        velocity.Value.z *= .8f;
                    }
                }
            }).ScheduleParallel();
    }
}
