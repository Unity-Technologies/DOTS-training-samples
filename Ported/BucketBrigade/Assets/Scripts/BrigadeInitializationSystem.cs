using System.Net;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public struct Brigade : IComponentData
{
    public float3 fireTarget;
    public Entity fireEntity;
    public Entity waterEntity;
    public Random random;
}

public struct BotConfig : IComponentData
{
    public float botSpeed;
}
public struct BrigadeGroup : IComponentData
{
    public Entity Value;
}
public struct TargetPosition : IComponentData
{
    public float3 Value;
}

public struct BotTypeScoop : IComponentData {}
public struct BotTypeToss : IComponentData {}

public struct TargetBucket : IComponentData
{
    public Entity Value;
}

public struct CarriedBucket : IComponentData
{
    public Entity Value;
}

// gross - should replace with tags
public struct EmptyPasserInfo : IComponentData
{
    public int ChainLength;
    public int ChainPosition;
}
public struct FullPasserInfo : IComponentData
{
    public int ChainLength;
    public int ChainPosition;
}

public struct NextBot : IComponentData
{
    public Entity Value;
}

public struct UtilityFunctions
{
    // check on just x/z
    public static bool FlatOverlapCheck(float3 a, float3 b)
    {
        return math.length((a - b).xz) < 0.1f;
    }

    public static float3 BotHeightCorrect(float3 position)
    {
        position.y = 0.25f;
        return position;
    }
    public static void SetupBot(EntityCommandBuffer cb, Entity instance, float3 position, float4 color, Entity brigadeEntity)
    {
        cb.SetComponent(instance, new Translation()
        {
            Value = position    
        });
        cb.AddComponent<Color>(instance);
        cb.SetComponent(instance, new Color()
        {
            Value = color
        });
        // this is maybe bad duplicated data? But otherwise we need to store this elsewhere
        cb.AddComponent<BrigadeGroup>(instance);
        cb.SetComponent(instance, new BrigadeGroup()
        {
            Value = brigadeEntity
        });
    }
    public static float3 GetChainPosition(int _index, int _chainLength, float3 _startPos, float3 _endPos){
        // adds two to pad between the SCOOPER AND THROWER
        float progress = (float) _index / _chainLength;
        float curveOffset = math.sin(progress * math.PI) * 1f;

        // get Vec2 data
        float2 heading = new float2(_startPos.x, _startPos.z) -  new float2(_endPos.x, _endPos.y);
        float distance = math.length(heading);
        float2 direction = heading / distance;
        float2 perpendicular = new float2(direction.y, -direction.x);

        return math.lerp(_startPos, _endPos, (float)_index / (float)_chainLength) + (new float3(perpendicular.x, 0f, perpendicular.y) * curveOffset);
    }
}

public class BrigadeInitializationSystem : SystemBase
{
    private EntityQuery m_riverEntityQuery;
    private EntityCommandBufferSystem ecbs;
    static public float botSpeed; 
    protected override void OnCreate()
    {
        ecbs = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        m_riverEntityQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<Water>(),
            },
            None = new []
            {
                ComponentType.ReadOnly<Bucket>(),
            }
        });
        var botConfig = EntityManager.CreateEntity();
        EntityManager.AddComponentData(botConfig, new BotConfig()
        {
            botSpeed = 3 // this should be pulled from the authoring data, not sure why it isn't
        });

    }

    protected override void OnUpdate()
    {
        var cb = ecbs.CreateCommandBuffer();
        // build a list of water entities
        var riverPositions = m_riverEntityQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        var riverEntities = m_riverEntityQuery.ToEntityArray(Allocator.TempJob);
        
        var job1 = Entities
            .ForEach((in Entity e, in BrigadeInitialization init, in BrigadeColor colors) =>
            {
                // create brigades 
                var random = new Random(1);
                for (int i = 0; i < init.brigadeCount; i++)
                {
                    var fireTarget = random.NextFloat3()*10.0f;
                    fireTarget.y = 0;
                    // find a water target
                    var waterTarget = Entity.Null;
                    var waterPosition = float3.zero;

                    int randomWaterindex = random.NextInt(0, riverEntities.Length);

                    waterTarget = riverEntities[randomWaterindex];
                    waterPosition = GetComponent<LocalToWorld>(waterTarget).Value.c3.xyz; 
                    
                    var brigade = cb.CreateEntity();
                    cb.AddComponent<Brigade>(brigade);
                    cb.SetComponent(brigade, new Brigade()
                    {
                        fireTarget = fireTarget,
                        waterEntity = waterTarget,
                        random = new Random(random.NextUInt())
                    });
                    
                    // add a tosser bot
                    var tosserBot = cb.Instantiate(init.bot);
                    cb.AddComponent<BotTypeToss>(tosserBot);
                    UtilityFunctions.SetupBot(cb, tosserBot, fireTarget, colors.tossColor, brigade);
                    
                    // add a scoop bot
                    var scooperBot = cb.Instantiate(init.bot);
                    
                    cb.AddComponent<BotTypeScoop>(scooperBot);
                    UtilityFunctions.SetupBot(cb, scooperBot, waterPosition, colors.scoopColor, brigade);
                    
                    Entity lastBot = Entity.Null;
                    for (var j = 0; j < init.emptyPassers; j++)
                    {
                        var instance = cb.Instantiate(init.bot);
                        var position = UtilityFunctions.GetChainPosition(j, init.emptyPassers, waterPosition, fireTarget);
                        UtilityFunctions.SetupBot(cb, instance, position, colors.emptyColor, brigade);
                        
                        cb.AddComponent<EmptyPasserInfo>(instance);
                        cb.SetComponent(instance, new EmptyPasserInfo()
                        {
                            ChainLength = init.emptyPassers,
                            ChainPosition = j
                        });
                        cb.AddComponent<TargetPosition>(instance);
                        cb.SetComponent(instance, new TargetPosition()
                        {
                            Value = float3.zero
                        });
                        
                        // the first empty passer receives from the tosser
                        if (j == 0)
                        {
                            cb.AddComponent<NextBot>(tosserBot);
                            cb.SetComponent(tosserBot, new NextBot() {Value = instance});
                        }
                        if (lastBot != Entity.Null)
                        {
                            cb.AddComponent<NextBot>(lastBot);
                            cb.SetComponent(lastBot, new NextBot() {Value = instance});
                        }
                        lastBot = instance;
                    }
                    lastBot = Entity.Null;

                    for (var j = 0; j < init.fullPassers; j++)
                    {
                        var instance = cb.Instantiate(init.bot);
                        float3 position = UtilityFunctions.GetChainPosition(j, init.fullPassers, fireTarget, waterPosition);
                        UtilityFunctions.SetupBot(cb, instance, position, colors.fullColor, brigade);
                        cb.AddComponent<FullPasserInfo>(instance);
                        cb.SetComponent(instance, new FullPasserInfo()
                        {
                            ChainLength = init.fullPassers,
                            ChainPosition = j
                        });
                        cb.AddComponent<TargetPosition>(instance);
                        cb.SetComponent(instance, new TargetPosition()
                        {
                            Value = float3.zero
                        });
                        
                        // the first full passer receives from the scooper
                        if (j == 0)
                        {
                            cb.AddComponent<NextBot>(scooperBot);
                            cb.SetComponent(scooperBot, new NextBot() {Value = instance});
                        }
                        else if (lastBot != Entity.Null)
                        {
                            cb.AddComponent<NextBot>(lastBot);
                            cb.SetComponent(lastBot, new NextBot() {Value = instance});
                        }
                        // the last full passer passes to the tosser
                        if (j == init.fullPassers - 1)
                        {
                            cb.AddComponent<NextBot>(instance);
                            cb.SetComponent(instance, new NextBot() {Value = tosserBot});
                        }
                        lastBot = instance;
                    }
                }
                // remove the setup data
                cb.DestroyEntity(e);
            }).Schedule(Dependency);
        riverPositions.Dispose(job1);
        riverEntities.Dispose(job1);
        ecbs.AddJobHandleForProducer(job1);
        Dependency = job1;
    }
}
