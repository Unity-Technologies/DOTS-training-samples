using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct TileCreationSystem : ISystem
{
    public int Width;
    public int Height;

    public void OnCreate(ref SystemState state)
    {
        Width = 100;
        Height = 100;
        state.RequireForUpdate<PrefabConfig>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        PrefabConfig prefabConfig = SystemAPI.GetSingleton<PrefabConfig>();

        Entity groundEntity = state.EntityManager.Instantiate(prefabConfig.TilePrefab);
        TransformAspect transform = SystemAPI.GetAspectRW<TransformAspect>(groundEntity);
        UniformScaleTransform temp = transform.LocalToWorld;
        temp.Scale = Width;
        transform.LocalToWorld = temp;

        Entity entity = state.EntityManager.CreateEntity();
        DynamicBuffer<TileBufferElement> tiles = state.EntityManager.AddBuffer<TileBufferElement>(entity);
        tiles.Capacity = Width * Height;

        for (int x = 0; x < Width; ++x)
        {
            for (int y = 0; y < Height; ++y)
            {
                // some alogrithim 
                tiles.Add(new TileBufferElement());
            }
        }

        // To Use
        // SystemAPI.GetSingletonBuffer<TileBufferElement>();

        state.Enabled = false;
    }
}
