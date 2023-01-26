using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using System.Diagnostics;
using Unity.Mathematics;

[BurstCompile]
public partial struct RockSystem : ISystem
{
    float totalTime;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        state.RequireForUpdate<WorldGrid>();
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
        var worldGrid = SystemAPI.GetSingleton<WorldGrid>();
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
                    int2 gridPoint = worldGrid.WorldToGrid(rock.Transform.LocalPosition);
                    worldGrid.SetTypeAt(gridPoint.x,gridPoint.y,0);

                    ecb.DestroyEntity(entity);
                    ecb.DestroyEntity(children[0].Value);
                }
            }
            totalTime = 0;
        }

    }

}
