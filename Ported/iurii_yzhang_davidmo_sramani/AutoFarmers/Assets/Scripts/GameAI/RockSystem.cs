using GameAI;
using Pathfinding;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class RockSystem : JobComponentSystem
{
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        // Cache the BeginInitializationEntityCommandBufferSystem in a field, so we don't have to create it every frame
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var dt = Time.deltaTime;
        var Cmd = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
     
        var health = GetComponentDataFromEntity<HealthComponent>(true);
        var rockSmashSpeed = 0.5f;
        var job = Entities
            .WithNone<AISubTaskTagComplete>()
            .WithReadOnly(health)
            .WithReadOnly(rockSmashSpeed)
//            .WithoutBurst()
            .ForEach((Entity entity, int entityInQueryIndex, in AISubTaskTagClearRock rock) =>
            {
                if (!health.Exists(rock.rockEntity)) {
                    Cmd.AddComponent<AISubTaskTagComplete>(entityInQueryIndex, entity);
                } else {
                    var newHealth = health[rock.rockEntity].Value - rockSmashSpeed * dt;
                    if (newHealth < 0.0) {
                        Cmd.DestroyEntity(entityInQueryIndex, rock.rockEntity);
                        Cmd.AddComponent<AISubTaskTagComplete>(entityInQueryIndex, entity);
                    } else {
                        Cmd.SetComponent(entityInQueryIndex, rock.rockEntity, new HealthComponent() {Value = newHealth});
                    }
                }
            }).Schedule(inputDeps);

        m_EntityCommandBufferSystem.AddJobHandleForProducer(job);
        return JobHandle.CombineDependencies(inputDeps, job);
    }
}