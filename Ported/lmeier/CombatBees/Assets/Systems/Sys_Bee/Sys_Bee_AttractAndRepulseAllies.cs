using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;


[UpdateAfter(typeof(Sys_Bee_ChaseTarget))]
public class Sys_Bee_AttractAndRepulseAllies : JobComponentSystem
{
    [BurstCompile]
    struct Sys_Bee_AttractAndRepulseAlliesJob : IJobForEach<C_Velocity, C_Random, Translation>
    {
        [ReadOnly] public ComponentDataFromEntity<Translation> TranslationData;
        [ReadOnly][DeallocateOnJobCompletion] public NativeArray<Entity> Teammates;

        [ReadOnly] public float dt;
        [ReadOnly] public float TeamAttraction;
        [ReadOnly] public float TeamRepulsion;

        public void Execute(ref C_Velocity Velocity, ref C_Random Rand, [ReadOnly] ref Translation Trans)
        {
            if(Teammates.Length == 0)
                return;
            
            int attractiveAlly = Rand.Generator.NextInt(0, Teammates.Length);
            int repulsiveAlly = Rand.Generator.NextInt(0, Teammates.Length);

            float3 attractivePos = TranslationData[Teammates[attractiveAlly]].Value;
            float3 repulsivePos = TranslationData[Teammates[repulsiveAlly]].Value;

            float attractDist = distance(attractivePos, Trans.Value);
            float repulseDist = distance(repulsivePos, Trans.Value);

            if(attractDist > 0f)
            {
                Velocity.Value += (attractivePos - Trans.Value) * (TeamAttraction * dt / attractDist);
            }
            if (repulseDist > 0f)
            {
                Velocity.Value -= (repulsivePos - Trans.Value) * (TeamRepulsion * dt / repulseDist);
            }

        }
    }


    private EntityQuery m_yellowGroup;
    private EntityQuery m_purpleGroup;

    protected override void OnCreate()
    {
        var queryDesc = new EntityQueryDesc
        {
            None = new ComponentType[] { ComponentType.ReadOnly<Tag_IsDead>(), ComponentType.ReadOnly<Tag_IsDying>() },
            All = new ComponentType[] {
                ComponentType.ReadOnly<Tag_Bee>(),
                ComponentType.ReadOnly<C_Shared_Team>(),
                ComponentType.ReadOnly<Translation>(),
                typeof(C_Velocity),
                typeof(C_Random)}
        };

        m_yellowGroup = GetEntityQuery(queryDesc);
        m_purpleGroup = GetEntityQuery(queryDesc);

    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var dependencies = inputDependencies;
        if (Sys_Bee_GetTarget.yellowEntities.IsCreated)
        {
            m_yellowGroup.SetFilter(new C_Shared_Team() {Team = Proxy_Bee.Team.YellowTeam});

            var yellowJob = new Sys_Bee_AttractAndRepulseAlliesJob()
            {
                TranslationData = GetComponentDataFromEntity<Translation>(true),
                Teammates = Sys_Bee_GetTarget.yellowEntities,
                dt = UnityEngine.Time.deltaTime,
                TeamAttraction = BeeManager.S.TeamAttraction,
                TeamRepulsion = BeeManager.S.TeamRepulsion
            }.Schedule(m_yellowGroup, inputDependencies);

            dependencies = yellowJob;
        }

        if (Sys_Bee_GetTarget.purpleEntities.IsCreated)
        {
            m_purpleGroup.SetFilter(new C_Shared_Team() {Team = Proxy_Bee.Team.PurpleTeam});
            var purpleJob = new Sys_Bee_AttractAndRepulseAlliesJob()
            {
                TranslationData = GetComponentDataFromEntity<Translation>(true),
                Teammates = Sys_Bee_GetTarget.purpleEntities,
                dt = UnityEngine.Time.deltaTime,
                TeamAttraction = BeeManager.S.TeamAttraction,
                TeamRepulsion = BeeManager.S.TeamRepulsion
            }.Schedule(m_purpleGroup, dependencies);

            dependencies = purpleJob;
        }

        // Now that the job is set up, schedule it to be run. 
        return dependencies;
    }
}
 