using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class RockDestroying : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var settings = GetSingleton<CommonSettings>();
        var data = GetSingletonEntity<CommonData>();
        var tileBuffer = GetBufferFromEntity<TileState>()[data];
        
        Entities.ForEach((Entity entity, ref Rock rock, ref NonUniformScale nonUniformScale) =>
            {
                nonUniformScale.Value = new float3(nonUniformScale.Value.x, rock.Health, nonUniformScale.Value.z);
                if (rock.Health <= 0)
                {
                    ecb.DestroyEntity(entity);
                    for (int x = rock.Position.x; x < rock.Position.x + rock.Size.x; x++)
                    {
                        for (int y = rock.Position.y; y < rock.Position.y + rock.Size.y; y++)
                        {
                            var linearIndex = x + y * settings.GridSize.x;
                            tileBuffer[linearIndex] = new TileState { Value = ETileState.Empty};
                            //var state = tileBuffer[linearIndex].Value;
                        }
                    }
                }
            }).Run();
        
        ecb.Playback(EntityManager);
    }
}