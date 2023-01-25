using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using System.Diagnostics;

[BurstCompile]
public partial struct RockSystem : ISystem
{
    float totalTime;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
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


        foreach( var rock in SystemAPI.Query<RockAspect>() )
        {
            rock.Transform.LocalScale = ( rock.Health / (float)maxHealth );
        }

        if ( totalTime > 0.5)
        {
            foreach( var (rock, children, entity) in SystemAPI.Query<RockAspect, DynamicBuffer<Child>>().WithEntityAccess())
            {
                if (rock.Health <= 0)
                {
                    ecb.DestroyEntity(entity);
                    ecb.DestroyEntity(children[0].Value);
                }
            }
            totalTime = 0;
        }

    }

}
