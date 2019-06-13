using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

public class Sys_Bee_GoHome : JobComponentSystem
{

    [RequireComponentTag(typeof(Tag_Bee), typeof(C_Holding))]
    struct Sys_Bee_GoHomeJob : IJobForEachWithEntity<C_Velocity, Translation>
    {
        [ReadOnly] public float CarryForce;
        [ReadOnly] public float3 Field;
        [ReadOnly] public int Team;
        [ReadOnly] public float dt;
        public EntityCommandBuffer.Concurrent ecb;

        public void Execute(Entity ent, int index, ref C_Velocity Velocity, [ReadOnly] ref Translation Pos)
        {
            //I don't know why the teams are backwards, but I just want it to work now.
            float3 targetPos = float3(-Field.x * 0.45f + Field.x * .9f * (1-Team), 0, Pos.Value.z);

            float dist = distance(targetPos, Pos.Value);
            Velocity.Value += (targetPos - Pos.Value) * (CarryForce * dt / dist);

            if(dist < 1f)
            {
                ecb.RemoveComponent(index, ent, typeof(C_Holding));
            }
        }
    }

    private EntityQuery m_yellowGroup;
    private EntityQuery m_purpleGroup;

    private EntityCommandBufferSystem m_entityCommandBufferSystem;

    protected override void OnCreate()
    {
        m_entityCommandBufferSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();

        var queryDesc = new EntityQueryDesc
        {
            All = new ComponentType[] {
                ComponentType.ReadOnly<Tag_Bee>(),
                ComponentType.ReadOnly<C_Shared_Team>(),
                ComponentType.ReadOnly<C_Holding>(),
                ComponentType.ReadOnly<Translation>(),
            typeof(C_Velocity)}
        };

        m_yellowGroup = GetEntityQuery(queryDesc);
        m_purpleGroup = GetEntityQuery(queryDesc);
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        m_yellowGroup.SetFilter(new C_Shared_Team() { Team = Proxy_Bee.Team.YellowTeam });

        var yellowJob = new Sys_Bee_GoHomeJob()
        {
            CarryForce = BeeManager.S.CarryForce,
            Field = Field.size,
            Team = (int)Proxy_Bee.Team.YellowTeam,
            dt = UnityEngine.Time.deltaTime,
            ecb = m_entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()

        }.Schedule(m_yellowGroup, inputDependencies);

        m_purpleGroup.SetFilter(new C_Shared_Team() { Team = Proxy_Bee.Team.PurpleTeam });
        var purpleJob = new Sys_Bee_GoHomeJob()
        {
            CarryForce = BeeManager.S.CarryForce,
            Field = Field.size,
            Team = (int)Proxy_Bee.Team.PurpleTeam,
            dt = UnityEngine.Time.deltaTime,
            ecb = m_entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()

        }.Schedule(m_purpleGroup, yellowJob);

        m_entityCommandBufferSystem.AddJobHandleForProducer(yellowJob);
        m_entityCommandBufferSystem.AddJobHandleForProducer(purpleJob);
        
        return purpleJob;
    }
}
 