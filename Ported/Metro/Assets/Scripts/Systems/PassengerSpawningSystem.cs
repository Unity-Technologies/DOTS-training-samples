using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[BurstCompile]
partial struct PassengerSpawningSystem : ISystem
{
    EntityQuery m_BaseColorQuery;
    EntityQuery m_Transform;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PassengersConfig>();
        m_BaseColorQuery = state.GetEntityQuery(ComponentType.ReadOnly<URPMaterialPropertyBaseColor>());
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        // throw new System.NotImplementedException();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<PassengersConfig>();

        var random = Random.CreateFromIndex(1234);
        var hue = random.NextFloat();

        URPMaterialPropertyBaseColor RandomColor()
        {
            hue = (hue + 0.618034005f) % 1;
            var color = UnityEngine.Color.HSVToRGB(hue, 1.0f, 1.0f);
            return new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)color };
        }

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var passengers = CollectionHelper.CreateNativeArray<Entity>(config.passengerCount, Allocator.Temp);
        ecb.Instantiate(config.passengerPrefab, passengers);

        var queryMask = m_BaseColorQuery.GetEntityQueryMask();

        foreach (var passenger in passengers)
        {
            float3 pos = new float3(random.NextFloat(-50, 50),
                0,
                random.NextFloat(-50.0f, 50.0f));
            ecb.SetComponent<LocalTransform>(passenger, LocalTransform.FromPosition(pos));
            ecb.SetComponentForLinkedEntityGroup(passenger, queryMask, RandomColor());
            var pts = new PostTransformScale { Value = float3x3.Scale(0.3f, random.NextFloat(0.4f, 1.1f), 0.3f) };
            ecb.SetComponent(passenger, pts);
        }

        state.Enabled = false;
    }
}
