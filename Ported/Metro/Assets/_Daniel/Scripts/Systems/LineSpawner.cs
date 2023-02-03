using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;

[UpdateAfter(typeof(/*BeginSimulationEntityCommandBufferSystem*/TransformSystemGroup))]
[BurstCompile]
partial struct LineSpawner : ISystem
{
    EntityQuery m_BaseColorQuery;
    ComponentLookup<WorldTransform> m_WorldTransformLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        m_WorldTransformLookup = state.GetComponentLookup<WorldTransform>(true);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        m_WorldTransformLookup.Update(ref state);

        var config = SystemAPI.GetSingleton<Config>();
        var random = Unity.Mathematics.Random.CreateFromIndex(1234);
        var hue = random.NextFloat();

        URPMaterialPropertyBaseColor RandomColor()
        {
            hue = (hue + 0.618034005f) % 1;
            var color = UnityEngine.Color.HSVToRGB(hue, 1.0f, 1.0f);
            return new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)color };
        }

        int systemCount = config.LineCount;
        int realLinesPerSystem = 4;

        NativeArray<Entity> lines = new NativeArray<Entity>(config.LineCount, Allocator.Persistent);
        for (int i = 0; i < systemCount; i++)
        {
            for (int j = 0; j < realLinesPerSystem; j++)
            {
                Entity e = state.EntityManager.Instantiate(config.LinePrefab);
                state.EntityManager.SetComponentData<Line>(e, new Line { LineColor = RandomColor().Value, Id = j, SystemId = i });
            }
        }

        state.Enabled = false;
    }
}

public class SpawningSystemGroup : ComponentSystemGroup
{

}
