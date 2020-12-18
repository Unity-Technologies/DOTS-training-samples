using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

public class DebugViewTileBuffer : SystemBase
{
    private EndSimulationEntityCommandBufferSystem m_ECB;
    
    protected override void OnCreate()
    {
        base.OnCreate();

        m_ECB = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }   
    
    protected override void OnUpdate()
    {
        var ecb = m_ECB.CreateCommandBuffer().AsParallelWriter();

        var settings = GetSingleton<CommonSettings>();
        var data = GetSingletonEntity<CommonData>();

        var tileBufferHandler = GetBufferFromEntity<TileState>(true);

        Dependency = Entities
            .WithAll<Tile>()
            .WithReadOnly(tileBufferHandler)
            .ForEach((Entity entity, ref URPMaterialPropertyBaseColor baseColor, in Translation translation) =>
            {
                var tileBuffer = tileBufferHandler[data];
                var tilePos = new int2((int)math.floor(translation.Value.x), (int)math.floor(translation.Value.z));
                var tileIndex = tilePos.x + tilePos.y * settings.GridSize.x;
                switch (tileBuffer[tileIndex].Value)
                {
                    case ETileState.Empty:
                        baseColor.Value = new float4(1);
                        break;
                    case ETileState.Tilled:
                        baseColor.Value = new float4(0.5f, 0.5f, 0.0f, 1.0f);
                        break;
                    case ETileState.Store:
                        baseColor.Value = new float4(0.0f, 0.0f, 0.7f, 1.0f);
                        break;
                    case ETileState.Rock:
                        baseColor.Value = new float4(1.0f, 0.0f, 0.0f, 1.0f);
                        break;
                    case ETileState.Seeded:
                        baseColor.Value = new float4(0.0f, 0.7f, 0.0f, 1.0f);
                        break;
                    case ETileState.Grown:
                        baseColor.Value = new float4(0.0f, 0.0f, 1.0f, 1.0f);
                        break;
                }
            }).ScheduleParallel(Dependency);
        
        m_ECB.AddJobHandleForProducer(Dependency);
    }
}