using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class RockDestroying : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var settingsEntity = GetSingletonEntity<Settings>();
        var tileBufferAccessor = GetBufferFromEntity<TileState>();
        var settings = GetComponentDataFromEntity<Settings>()[settingsEntity];
        
        Entities.ForEach((Entity entity, ref Rock rock, ref NonUniformScale nonUniformScale) =>
            {
                var tileBuffer = tileBufferAccessor[settingsEntity];
                
                nonUniformScale.Value = new float3(nonUniformScale.Value.x, rock.Health, nonUniformScale.Value.z);
                if (rock.Health <= 0)
                {
                    ecb.DestroyEntity(entity);
                    for (int x = rock.Position.x; x < rock.Position.x + rock.Size.x; x++)
                    {
                        for (int y = rock.Position.y; y < rock.Position.y + rock.Size.y; y++)
                        {
                            var linearIndex = x + y * settings.GridSize.x;
                            tileBuffer[linearIndex] = new TileState { Value = TileStates.Empty};
                            //var state = tileBuffer[linearIndex].Value;
                        }
                    }
                }
            }).Run();
        
        ecb.Playback(EntityManager);
    }
}