using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

/// <summary>
/// For each arm looking to grab a rock (i.e. having a HandleIdle component)
///     For each rock that is available
///         Find the nearest rock.
///         If any, the hand goes to grabbing state (HandleIdle -> HandGrabbingRock)
///         The rock is unchanged, meaning several hands can try to reach for the same rock in parallel
/// </summary>
public class ProjectileSelectionSystem : SystemBase
{
    private EntityQuery m_AvailableRocksQuery;

    protected override void OnCreate()
    {
        m_AvailableRocksQuery = EntityManager.CreateEntityQuery(ComponentType.ReadOnly<Rock>(),
            ComponentType.ReadOnly<Available>());
    }
    
    protected override void OnUpdate()
    {
        EntityCommandBufferSystem sys = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        var ecb = sys.CreateCommandBuffer().AsParallelWriter();
        
        var translations = GetComponentDataFromEntity<Translation>();
        var sortedArms = GetBufferFromEntity<SortedArm>()[GetSingletonEntity<SortedArm>()];
        var handsIdle = GetComponentDataFromEntity<HandIdle>();

        var parameters = GetSingleton<SimulationParameters>();

        Dependency = Entities
            .WithAll<Rock, Available>()
            .WithReadOnly(sortedArms)
            .WithReadOnly(handsIdle)
            .WithReadOnly(translations)
            .ForEach((Entity entity, int entityInQueryIndex,
                in Translation translation, in Velocity velocity) =>
            {
                var futurePos = translation.Value + velocity.Value;

                var grabDist = 5.1f;
                float minBound = futurePos.x - grabDist;
                float maxBound = futurePos.x + grabDist;

                var minIndex = math.max((int) math.floor(minBound / parameters.ArmSeparation), 0);
                var maxIndex = math.min((int) math.floor(maxBound / parameters.ArmSeparation),
                    sortedArms.Length - 1);

                for (var i = minIndex; i <= maxIndex; ++i)
                {
                    var armEntity = sortedArms[i].ArmEntity;
                    if (!handsIdle.HasComponent(armEntity))
                    {
                        // arm is not looking for a rock
                        continue;
                    }

                    var armPos = translations[sortedArms[i].ArmEntity];
                    if (math.distancesq(armPos.Value, translation.Value) <= grabDist * grabDist)
                    {
                        // Set target rock to reach
                        // (doesn't mean the rock will actually be grabbed since another arm might compete for it)
                        ecb.SetComponent(entityInQueryIndex, armEntity, new TargetRock()
                        {
                            RockEntity = entity
                        });
                        
                        // Go to grab rock state
                        Utils.GoToState<HandIdle, HandGrabbingRock>(ecb, entityInQueryIndex, armEntity);

                        break;
                    }
                }
            }).ScheduleParallel(Dependency);

        sys.AddJobHandleForProducer(Dependency);
    }
}
