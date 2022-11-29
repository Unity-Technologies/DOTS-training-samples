using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[UpdateAfter(typeof(AntMovementSystem))]
//[BurstCompile]
public partial struct PheromoneSpawningSystem : ISystem
{
    //[BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        
        // TODO: can query config for map size?

        var e = state.EntityManager.CreateSingletonBuffer<PheromoneMap>();
        var map = state.EntityManager.GetBuffer<PheromoneMap>(e);
        map.Resize( 64 * 64, NativeArrayOptions.ClearMemory);
    }

    //[BurstCompile]
    public void OnDestroy(ref SystemState state) {}

    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // TODO: config
        float2 planePosition = new float2(10, -10);
        float2 planeExtent = new float2(10, 10); // TODO: get from the game object??
        float2 planeHalfExtent = planeExtent / 2;
        int2 textureSize = new int2(PheromoneDisplaySystem.PheromoneTextureSizeX, PheromoneDisplaySystem.PheromoneTextureSizeX); // TODO: Config???
        
        var pheromoneMap = SystemAPI.GetSingletonBuffer<PheromoneMap>();
        foreach( var (transform, ant) in SystemAPI.Query<TransformAspect, Ant>())
        {
            float2 pos = transform.LocalPosition.xz;
            float2 posNormalized = (pos - planePosition + planeHalfExtent) / planeExtent;
            int2 posTex = new int2(posNormalized * textureSize);
            
            if ((uint)posTex.x < textureSize.x && (uint)posTex.y < textureSize.y)
            {
                int bufIndex = posTex.x + posTex.y * textureSize.x;
                ref var cell = ref pheromoneMap.ElementAt(bufIndex);
                cell.amount += 0.1f; // TODO: config
            }
        }
    }
}
