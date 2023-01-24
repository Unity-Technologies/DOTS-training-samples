using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;

[BurstCompile]
public partial struct RockSystem : ISystem
{
    float totalTime;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        totalTime = 0;
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        totalTime += SystemAPI.Time.DeltaTime;

        var maxHealth = RockAuthoring.MAX_ROCK_HEALTH;


        foreach( var (tranform, rock) in SystemAPI.Query<TransformAspect, Rock>().WithAll<Rock>() )
        {
            tranform.LocalScale = ( rock.RockHealth / (float)maxHealth );
            //Read the rock health
            //Adjust the scale from 1 to 0 based on the health
        }

        if ( totalTime > 0.5)
        {
            foreach( var (rock, children, entity) in SystemAPI.Query<RefRW<Rock>, DynamicBuffer<Child>>().WithEntityAccess())
            {
                if (rock.ValueRW.RockHealth <= 0)
                {
                    ecb.DestroyEntity(entity);
                    ecb.DestroyEntity(children[0].Value);
                }
            }
            totalTime = 0;
        }

    }

}
