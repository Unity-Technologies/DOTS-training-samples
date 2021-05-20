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
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
        var screenSize = GetSingleton<ScreenSize>();
        var simSpeed = GetSingleton<SimulationSpeed>();

        var initJob = Entities
            .ForEach((Entity entity, ref NonUniformScale scale, in PheromoneMap map, in Respawn respawn) =>
            {
                ecb.SetComponent(entity, new NonUniformScale()
                {
                    Value = new float3(screenSize.Value, screenSize.Value, 1.0f)
                });
                
                ecb.RemoveComponent<Respawn>(entity);
            }).Schedule(Dependency);
    
        if (Time.ElapsedTime > lastIntervalTime + (pheromoneIntervalTime / simSpeed.Value) &&
            !PresentCustomTextureQuery.IsEmpty)
        {
            var pheromoneMapEntity = GetSingletonEntity<PheromoneMap>();
            var pheromoneMap = GetComponent<PheromoneMap>(pheromoneMapEntity);
            var multiplySize = (float)pheromoneMap.gridSize / (float)screenSize.Value;
            var pheromoneBuffer = GetBuffer<Pheromone>(pheromoneMapEntity);
            
            lastIntervalTime = Time.ElapsedTime;
            var colorJob = Entities
                .WithAll<Ant>()
                .WithNativeDisableParallelForRestriction(pheromoneBuffer)
                .ForEach((Entity entity, in Translation pos) =>
                {
                    float2 normPos = (pos.Value.xy +
                                       new float2(screenSize.Value / 2, screenSize.Value / 2))
                                      * multiplySize;

                    for (int pX = -1; pX <= 1; pX++)
                    {
                        for (int pY = -1; pY <= 1; pY++)
                        {
                            int x, y;
                            x = math.clamp((int)math.floor(normPos.x) + pX, 0, pheromoneMap.gridSize - 1);
                            y = math.clamp((int)math.floor(normPos.y) + pY, 0, pheromoneMap.gridSize - 1);
                            int index = y * pheromoneMap.gridSize + x;
                            
                            float newCol = pheromoneBuffer[index].Value;
                            newCol = math.clamp(newCol + 0.002f, 0.0f, 1.0f);
                            pheromoneBuffer[index] = new Pheromone()
                            {
                                Value = newCol
                            };
                        }
                    }
                }).ScheduleParallel(initJob);
            
            colorJob.Complete();
        }
                    
        initJob.Complete();
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}