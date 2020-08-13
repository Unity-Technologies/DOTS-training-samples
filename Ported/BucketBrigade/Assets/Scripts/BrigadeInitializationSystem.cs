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
    public float3 waterTarget;
    public int length;
    public Random random;
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
public class BrigadeInitializationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithStructuralChanges()
            .ForEach((in Entity e, in BrigadeInitialization init, in BrigadeColor colors) =>
            {
                
                // create brigades 
                var random = new Random(1);
                for (int i = 0; i < init.brigadeCount; i++)
                {
                    var fireTarget = random.NextFloat3()*10.0f;
                    fireTarget.y = 0;
                    var waterTarget = random.NextFloat3()*10.0f;
                    waterTarget.y = 0;
                    var brigade = EntityManager.CreateEntity();
                    EntityManager.AddComponentData(brigade, new Brigade()
                    {
                        fireTarget = fireTarget,
                        waterTarget = waterTarget,
                        random = new Random(random.NextUInt())
                    });
                    
                    // add a tosser bot
                    var tosserBot = EntityManager.Instantiate(init.bot);
                    EntityManager.AddComponent<BotTypeToss>(tosserBot);
                    SetupBot(tosserBot, waterTarget, colors.tossColor, brigade);
                    
                    // add a scoop bot
                    var scooperBot = EntityManager.Instantiate(init.bot);
                    EntityManager.AddComponent<BotTypeScoop>(scooperBot);
                    SetupBot(scooperBot, fireTarget, colors.scoopColor, brigade);
                    Entity lastBot = Entity.Null;
                    for (var j = 0; j < init.emptyPassers; j++)
                    {
                        var instance = EntityManager.Instantiate(init.bot);
                        var position = GetChainPosition(j, init.emptyPassers, waterTarget, fireTarget);
                        SetupBot(instance, position, colors.emptyColor, brigade);
                        EntityManager.AddComponentData(instance, new EmptyPasserInfo()
                        {
                            ChainLength = init.emptyPassers,
                            ChainPosition = j
                        });
                        EntityManager.AddComponentData(instance, new TargetPosition()
                        {
                            Value = float3.zero
                        });
                        
                        // the first empty passer receives from the tosser
                        if (j == 0)
                        {
                            EntityManager.AddComponentData(tosserBot, new NextBot() {Value = instance});
                        }
                        if (lastBot != Entity.Null)
                        {
                            EntityManager.AddComponentData(lastBot, new NextBot() {Value = instance});
                        }
                        lastBot = instance;
                    }
                    lastBot = Entity.Null;

                    for (var j = 0; j < init.fullPassers; j++)
                    {
                        var instance = EntityManager.Instantiate(init.bot);
                        float3 position = GetChainPosition(j, init.fullPassers, fireTarget, waterTarget);
                        SetupBot(instance, position, colors.fullColor, brigade);
                        EntityManager.AddComponentData(instance, new FullPasserInfo()
                        {
                            ChainLength = init.fullPassers,
                            ChainPosition = j
                        });
                        EntityManager.AddComponentData(instance, new TargetPosition()
                        {
                            Value = float3.zero
                        });
                        
                        // the first full passer receives from the scooper
                        if (j == 0)
                        {
                            EntityManager.AddComponentData(scooperBot, new NextBot() {Value = instance});
                        }
                        else if (lastBot != Entity.Null)
                        {
                            EntityManager.AddComponentData(lastBot, new NextBot() {Value = instance});
                        }
                        // the last full passer passes to the tosser
                        if (j == init.fullPassers - 1)
                        {
                            EntityManager.AddComponentData(instance, new NextBot() {Value = tosserBot});
                        }
                        lastBot = instance;
                    }
                }
                // remove the setup data
                EntityManager.DestroyEntity(e);
            }).Run();
    }

    private void SetupBot(Entity instance, float3 position, float4 color, Entity brigadeEntity)
    {
        SetComponent(instance, new Translation()
        {
            Value = position    
        });
        EntityManager.AddComponentData(instance, new BotColor()
        {
            Value = color
        });
        // this is maybe bad duplicated data? But otherwise we need to store this elsewhere
        EntityManager.AddComponentData(instance, new BrigadeGroup()
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
