using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct BrigadeId : ISharedComponentData
{
    public int Value;
}
public class BrigadeInitializationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithStructuralChanges()
            .ForEach((in Entity e, in BrigadeInitialization init, in BrigadeColor colors) =>
            {
                for (int i = 0; i < init.brigadeCount; i++)
                {
                    for (int j = 0; j < init.emptyPassers; j++)
                    {
                        var instance = EntityManager.Instantiate(init.bot);
                        SetComponent(instance, new Translation()
                        {
                            Value = GetChainPosition(j, init.emptyPassers, float3.zero, new float3(10, 0, 0))    
                        });
                        EntityManager.AddComponentData(instance, new BotColor()
                        {
                            Value = colors.emptyColor
                        });
                        EntityManager.AddSharedComponentData(instance, new BrigadeId()
                        {
                            Value = i
                        });
                    }
                    for (int j = 0; j < init.fullPassers; j++)
                    {
                        var instance = EntityManager.Instantiate(init.bot);
                        SetComponent(instance, new Translation()
                        {
                            Value = GetChainPosition(j, init.fullPassers,new float3(10, 0, 0), float3.zero)    
                        });
                        EntityManager.AddComponentData(instance, new BotColor()
                        {
                            Value = colors.fullColor
                        });
                        EntityManager.AddSharedComponentData(instance, new BrigadeId()
                        {
                            Value = i
                        });
                    }
                }
                // remove the setup data
                EntityManager.DestroyEntity(e);
            }).Run();
    }
    static float3 GetChainPosition(int _index, int _chainLength, float3 _startPos, float3 _endPos){
        // adds two to pad between the SCOOPER AND THROWER
        float progress = (float) _index / _chainLength;
        float curveOffset = math.sin(progress * math.PI) * 1f;

        // get Vec2 data
        float2 heading = new float2(_startPos.x, _startPos.z) -  new float2(_endPos.x, _endPos.y);
        float distance = math.length(heading);
        float2 direction = heading / distance;
        float2 perpendicular = new float2(direction.y, -direction.x);

        //Debug.Log("chain progress: " + progress + ",  curveOffset: " + curveOffset);
        return math.lerp(_startPos, _endPos, (float)_index / (float)_chainLength) + (new float3(perpendicular.x, 0f, perpendicular.y) * curveOffset);
    }
}
