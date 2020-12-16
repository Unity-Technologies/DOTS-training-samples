using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class RockDestroying : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var tileBufferAccessor = GetBufferFromEntity<TileState>();
        
        Entities.ForEach((Entity entity, ref Rock rock, ref NonUniformScale nonUniformScale) =>
            {
                nonUniformScale.Value = new float3(nonUniformScale.Value.x, rock.Health, nonUniformScale.Value.z);
                if (rock.Health <= 0)
                {
                    ecb.DestroyEntity(entity);
                    for (int i = rock.Position.x; i < rock.Position.x + rock.Size.x; i++)
                    {
                        for (int j = rock.Position.y; j < rock.Position.y + rock.Size.y; j++)
                        {
                            //tileBufferAccessor[]
                        }
                    }
                }
            }).Run();
        
        ecb.Playback(EntityManager);
    }
}