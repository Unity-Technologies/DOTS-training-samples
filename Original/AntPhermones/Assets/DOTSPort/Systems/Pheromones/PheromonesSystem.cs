using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct PheromonesSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<AntData>();
        state.RequireForUpdate<PheromoneBufferElement>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var globalSettings = SystemAPI.GetSingleton<GlobalSettings>();
        var pheromoneBufferElement = SystemAPI.GetSingletonBuffer<PheromoneBufferElement>();
        
        var pheromoneDropJob = new PheromoneDropJob
        {
            GlobalSettings = globalSettings,
            Pheromones = pheromoneBufferElement,
            DeltaTime = SystemAPI.Time.DeltaTime,
        };
        
        state.Dependency = pheromoneDropJob.ScheduleParallel(state.Dependency);
        state.Dependency.Complete(); // works like await

        var pheromoneDecreaseJob = new PheromoneDecreaseJob
        {
            Pheromones = pheromoneBufferElement.AsNativeArray(),
            TrailDecay = 1 - (globalSettings.TrailDecay * SystemAPI.Time.DeltaTime)
        };
        
        state.Dependency = pheromoneDecreaseJob.Schedule(pheromoneBufferElement.Length, 100, state.Dependency);
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    
    [BurstCompile]
    public partial struct PheromoneDecreaseJob : IJobParallelFor
    {
        public float TrailDecay;
        public NativeArray<PheromoneBufferElement> Pheromones;
        
        public void Execute(int index)
        {
            Pheromones[index] *= TrailDecay;
        }
    }
    
    [BurstCompile]
    public partial struct PheromoneDropJob : IJobEntity
    {
        public GlobalSettings GlobalSettings;
        public float DeltaTime;

        [NativeDisableParallelForRestriction]
        public DynamicBuffer<PheromoneBufferElement> Pheromones;
        
        void Execute(ref AntData ant, ref LocalTransform localTransform)
        {
            float excitement = .3f;
            if (ant.HoldingResource) 
            {
                excitement = 1f;
            }
            excitement *= ant.Speed / GlobalSettings.AntSpeed;
            int x = (int)math.floor(localTransform.Position.x);
            int y = (int)math.floor(localTransform.Position.y);
            
            if (x < 0 || y < 0 || x >= GlobalSettings.MapSizeX || y >= GlobalSettings.MapSizeY)
                return;
            
            int index = PheromoneIndex(x , y, GlobalSettings.MapSizeX);

            Pheromones[index] += (GlobalSettings.TrailAddSpeed * excitement * DeltaTime) * (1f - Pheromones[index]);
            if (Pheromones[index] > 1f)
            {
                Pheromones[index] = 1;
            }
        }
    }
    
    public static int PheromoneIndex(int x, int y, int mapSizeX) => x + y * mapSizeX;
}



