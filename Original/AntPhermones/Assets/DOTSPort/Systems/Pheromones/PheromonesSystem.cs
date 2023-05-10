using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

//[UpdateInGroup(typeof(PresentationSystemGroup))]
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
            MapSizeX = globalSettings.MapSizeX,
            MapSizeY = globalSettings.MapSizeY,
            TotalMapSize = globalSettings.MapSizeX * globalSettings.MapSizeY,
            Pheromones = pheromoneBufferElement,
            TrailAddSpeed = 1f, // TODO: make it in setting
            DeltaTime = SystemAPI.Time.DeltaTime,
        };
        
        var handle = pheromoneDropJob.Schedule(state.Dependency);
        handle.Complete();
        
        var pheromoneDecreaseJob = new PheromoneDecreaseJob
        {
            Pheromones = pheromoneBufferElement,
            TrailDecay = 1 - (0.01f * SystemAPI.Time.DeltaTime) // TODO: make it in setting
        };

        
        pheromoneDecreaseJob.Schedule(handle).Complete();
    }
    
    [BurstCompile]
    public partial struct PheromoneDecreaseJob : IJobEntity
    {
        public float TrailDecay;
        public DynamicBuffer<PheromoneBufferElement> Pheromones;
        
        [BurstCompile]
        void Execute()
        {
            for (int i = 0; i < Pheromones.Length; i++)
            {
                Pheromones[i] *= TrailDecay;
            }
        }
    }
    
    [BurstCompile]
    public partial struct PheromoneDropJob : IJobEntity
    {
        public int MapSizeX;
        public int MapSizeY;
        public int TotalMapSize;
        public float DeltaTime;
        public float TrailAddSpeed;

        // [NativeDisableParallelForRestriction]
        public DynamicBuffer<PheromoneBufferElement> Pheromones;

        [BurstCompile]
        void Execute(ref AntData ant, ref LocalTransform localTransform)
        {
            float excitement = .3f;
            if (ant.HoldingResource) 
            {
                excitement = 1f;
            }
            excitement *= ant.Speed / 1; // TODO: put `1` into the settings - `antSpeed`
            int x = (int)math.floor(localTransform.Position.x);
            int y = (int)math.floor(localTransform.Position.y);
            
            if (x < 0 || y < 0 || x >= MapSizeX || y >= MapSizeY)
                return;
            
            excitement = 1;
            int index = PheromoneIndex(x , y, TotalMapSize);

            Pheromones[index] += (TrailAddSpeed * excitement * DeltaTime);//*(1f - Pheromones[index]);
            if (Pheromones[index] > 1f)
            {
                Pheromones[index] = 1;
            }
        }
    }
    
    // TODO: is that correct if we have different size of x and y?
    static int PheromoneIndex(int x, int y, int totalMapSize) => x + y * totalMapSize;
}



