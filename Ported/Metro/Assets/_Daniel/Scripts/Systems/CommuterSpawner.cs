using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[UpdateAfter(typeof(LineSpawner))]
[BurstCompile]
partial struct CommuterSpawner : ISystem
{
    EntityQuery m_BaseColorQuery;

    public void OnCreate(ref SystemState state)
    {
        // This system should not run before the Config singleton has been loaded.
        state.RequireForUpdate<Config>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        var random = Random.CreateFromIndex(1234);
        var hue = random.NextFloat();

        URPMaterialPropertyBaseColor RandomColor()
        {
            hue = (hue + 0.618034005f) % 1;
            var color = UnityEngine.Color.HSVToRGB(hue, 1.0f, 1.0f);
            return new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)color };
        }

        var commuters = state.EntityManager.Instantiate(config.CommuterPrefab, config.CommuterCount, Allocator.Persistent);

        foreach (var commuter in commuters)
        {
            var position = new float3();
            position.xz = random.NextFloat2() * 4;

            state.EntityManager.SetComponentData<URPMaterialPropertyBaseColor>(commuter, RandomColor());
            state.EntityManager.SetComponentData<LocalTransform>(commuter, new LocalTransform { Position = position, Scale = 1 });
        }

        state.Enabled = false;
    }
}
