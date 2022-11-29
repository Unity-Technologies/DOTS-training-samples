using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;


[BurstCompile]
partial struct SpawnerSystem : ISystem
{
    [ReadOnly] public ComponentLookup<LocalTransform> LocalTransformFromEntity;
    EntityQuery m_BaseColorQuery;

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
        var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var config = SystemAPI.GetSingleton<Config>();
        m_BaseColorQuery = state.GetEntityQuery(ComponentType.ReadOnly<URPMaterialPropertyBaseColor>());
        var random = Random.CreateFromIndex(1234);

        for (int i = 0; i < config.PlatformCountPerStation; i++)
        {
            Entity platform = state.EntityManager.Instantiate(config.PlatformPrefab);

            var spawnLocalToWorld = new LocalTransform();
            spawnLocalToWorld.Scale = 1;
            spawnLocalToWorld.Position = new Unity.Mathematics.float3(9*i, 0, 0);
            spawnLocalToWorld.Rotation = Unity.Mathematics.quaternion.EulerXYZ(0, 0, 0);

            ecb.SetComponent<LocalTransform>(platform, spawnLocalToWorld);

            Entity rail = ecb.Instantiate(config.RailsPrefab);
            ecb.SetComponent<LocalTransform>(rail, spawnLocalToWorld);

            var hue = random.NextFloat();
            URPMaterialPropertyBaseColor RandomColor()
            {
                hue = (hue + 0.618034005f) % 1;
                var color = UnityEngine.Color.HSVToRGB(hue, 1.0f, 1.0f);
                return new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)color };
            }

            Entity person = state.EntityManager.Instantiate(config.PersonPrefab);
            ecb.SetComponent<LocalTransform>(person, spawnLocalToWorld);
            var queryMask = m_BaseColorQuery.GetEntityQueryMask();
            ecb.SetComponentForLinkedEntityGroup(person, queryMask, RandomColor());
        }

      //  var persons = CollectionHelper.CreateNativeArray<Entity>(config.PersonCount, Allocator.Temp);
      //  ecb.Instantiate(config.PersonPrefab, persons);
        state.Enabled = false;
    }
}