using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class PheromoneSpawnerSystem : SystemBase
{
    public float pheromoneIntervalTime = 0.05f;

    private EntityQuery PresentCustomTextureQuery;
    private double lastIntervalTime = 0.0f;

    protected override void OnCreate()
    {
        PresentCustomTextureQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] {typeof(PheromoneMap), typeof(Pheromone), typeof(RenderMesh)},
        });
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        var screenSize = GetSingleton<ScreenSize>();
        var simSpeed = GetSingleton<SimulationSpeed>();

        Entities
            .ForEach((Entity entity, ref NonUniformScale scale, in PheromoneMap map, in Respawn respawn) =>
            {
                ecb.SetComponent(entity, new NonUniformScale()
                {
                    Value = new float3(screenSize.Value, screenSize.Value, 1.0f)
                });
                
                ecb.RemoveComponent<Respawn>(entity);
            }).Run();
    
        if (Time.ElapsedTime > lastIntervalTime + (pheromoneIntervalTime / simSpeed.Value) &&
            !PresentCustomTextureQuery.IsEmpty)
        {
            var pheromoneMapEntity = GetSingletonEntity<PheromoneMap>();
            var pheromoneMap = GetComponent<PheromoneMap>(pheromoneMapEntity);
            var multiplySize = (float)pheromoneMap.gridSize / (float)screenSize.Value;
            var pheromoneBuffer = GetBuffer<Pheromone>(pheromoneMapEntity);
            
            lastIntervalTime = Time.ElapsedTime;
            Entities
                .WithAll<Ant>()
                .ForEach((Entity entity, in Translation pos) =>
                {
                    float2 normPos = (pos.Value.xy +
                                       new float2(screenSize.Value / 2, screenSize.Value / 2))
                                      * multiplySize;
                    int x, y;
                    x = (int)math.floor(normPos.x);
                    y = (int)math.floor(normPos.y);
                    int index = y * pheromoneMap.gridSize + x;

                    float newCol = pheromoneBuffer[index].Value;
                    newCol = math.clamp(newCol + 0.001f, 0.0f, 1.0f);
                    pheromoneBuffer[index] = new Pheromone()
                    {
                        Value = newCol
                    };
                }).Run();
            
            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}