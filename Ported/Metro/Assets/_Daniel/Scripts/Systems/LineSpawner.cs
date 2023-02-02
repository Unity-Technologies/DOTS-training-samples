using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(TransformSystemGroup))]
[BurstCompile]
partial struct LineSpawner : ISystem
{
    EntityQuery m_BaseColorQuery;
    ComponentLookup<WorldTransform> m_WorldTransformLookup;


    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        m_WorldTransformLookup = state.GetComponentLookup<WorldTransform>(true);
    }

    public void OnDestroy(ref SystemState state)
    {
    }

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

        NativeArray<Entity> lines = new NativeArray<Entity>(config.LineCount, Allocator.Persistent);
        for (int i = 0; i < config.LineCount; i++)
        {
            Entity e = state.EntityManager.Instantiate(config.LinePrefab);
            state.EntityManager.AddComponent<Line>(e);
            state.EntityManager.SetComponentData<Line>(e, new Line { LineColor = RandomColor().Value, Id = i + 1 });
        }
        //for (int i = 0; i < config.LineCount; i++)
        //{
        //    Entity e = state.EntityManager.Instantiate(config.LinePrefab,lines);
        //    state.EntityManager.AddComponent<Line>(e);
        //    state.EntityManager.SetComponentData<Line>(e, new Line { LineColor = RandomColor().Value, Id = i });
        //}
        state.Enabled = false;
    }
}

public class SpawningSystemGroup : ComponentSystemGroup
{

}
