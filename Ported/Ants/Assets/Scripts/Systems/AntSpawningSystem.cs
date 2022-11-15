using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


[BurstCompile]
public partial struct AntSpawningSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
     
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        var rand = new Random(123);
        
        var config = SystemAPI.GetSingleton<Config>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var ants = CollectionHelper.CreateNativeArray<Entity>(config.Amount, Allocator.Temp);
        ecb.Instantiate(config.AntPrefab, ants);

        foreach (var ant in ants)
        {
            var antComponent = new Ant()
            {
                Position = float2.zero,
                Speed = 5,
                Angle = rand.NextFloat(0f, 360f),
                HasFood = false
            };
            ecb.AddComponent(ant,antComponent);
        }
       
       state.Enabled = false;
    }
}