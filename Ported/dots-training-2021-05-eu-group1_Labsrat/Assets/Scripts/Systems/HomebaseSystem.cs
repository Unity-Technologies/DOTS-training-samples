using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public struct HomebaseData
{
    public Entity playerEntity;
    public float2 position;
}

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public class HomebaseSystem : SystemBase
{
    EntityCommandBufferSystem m_EcbSystem;
    NativeList<HomebaseData> m_HomeBases;

    protected override void OnCreate()
    {
        m_EcbSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        m_HomeBases = new NativeList<HomebaseData>(Allocator.Persistent);
    }

    protected override void OnUpdate()
    {
        if (TryGetSingleton(out GameConfig gameConfig))
        {
            var homeBases = m_HomeBases;
            if (homeBases.IsEmpty)
            {
                Entities
                    .ForEach((Entity playerEntity, in Homebase homebase, in Translation translation) =>
                    {
                        homeBases.Add(new HomebaseData()
                        {
                            playerEntity = homebase.PlayerEntity,
                            position = new float2(translation.Value.x, translation.Value.z)
                        });
                    }).Run();
            }

            var ecb = m_EcbSystem.CreateCommandBuffer();
            var scoreData = GetComponentDataFromEntity<Score>();

            Entities
                .WithAll<Mouse>()
                .WithReadOnly(homeBases)
                .ForEach((Entity mouseEntity, in Translation translation, in Direction direction) =>
            {
                float2 mousePos = new float2(translation.Value.x, translation.Value.z);
                foreach (var homebase in homeBases)
                {
                if (Utils.SnapTest(mousePos, homebase.position, direction.Value)
                    && math.distancesq(mousePos, homebase.position) < 1)
                    {
                        Score score = scoreData[homebase.playerEntity];
                        score.Value += 1;
                        scoreData[homebase.playerEntity] = score;
                        
                        ecb.DestroyEntity(mouseEntity);
                    }
                }
            }).Schedule();
            
            Entities
                .WithAll<Cat>()
                .WithReadOnly(homeBases)
                .ForEach((Entity catEntity, in Translation translation, in Direction direction) =>
            {
                float2 catPos = new float2(translation.Value.x, translation.Value.z);
                foreach (var homebase in homeBases)
                {
                    if (Utils.SnapTest(catPos, homebase.position, direction.Value)
                    && math.distancesq(catPos, homebase.position) < 1)
                    {
                        Score score = scoreData[homebase.playerEntity];
                        score.Value = math.max(0, score.Value-30);
                        scoreData[homebase.playerEntity] = score;
                        ecb.DestroyEntity(catEntity);
                    }
                }
            }).Schedule();
            
            m_EcbSystem.AddJobHandleForProducer(Dependency);
        }
    }

    protected override void OnDestroy()
    {
        m_HomeBases.Dispose();
    }
}
