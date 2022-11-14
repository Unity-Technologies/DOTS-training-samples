using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
public partial struct MazeGeneratorSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameConfig>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var gameConfig = SystemAPI.GetSingleton<GameConfig>();
        var gameConfigEntity = SystemAPI.GetSingletonEntity<GameConfig>();
        
        // create grid dynamic buffer
        var gridBuffer = state.EntityManager.AddBuffer<Grid>(gameConfigEntity);
        gridBuffer.Resize(gameConfig.mazeSize * gameConfig.mazeSize, NativeArrayOptions.ClearMemory);
        
        // spawn floor
        var floorEntity = state.EntityManager.Instantiate(gameConfig.tile);
        state.EntityManager.SetComponentData(floorEntity, new LocalToWorldTransform
        {
            Value = UniformScaleTransform.FromScale(gameConfig.cellSize * gameConfig.mazeSize)
        });

        state.Enabled = false;
    }
}