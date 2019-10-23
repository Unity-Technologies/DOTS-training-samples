using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public struct PlatformTransforms : IComponentData
{
    public BlobAssetReference<PlatformsTrBlob> value;
}

public struct PlatformsTrBlob
{
    public BlobArray<float3> translations;
    public BlobArray<quaternion> rotations;
}

public struct SpawnPlatforms : IComponentData
{
    public Entity platformPrefab;
}

[UpdateInGroup(typeof(GameObjectDeclareReferencedObjectsGroup))]
class MetroPlatformReferenceDeclaration : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Metro metro) =>
        {
            DeclareReferencedPrefab(metro.prefab_platform);
        });
    }
}

public class SpawnPlatformsSystem : JobComponentSystem
{
    EntityCommandBufferSystem m_Barrier;

    protected override void OnCreate()
    {
        m_Barrier = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new SpawnPlatformsJob{ cmdBuffer = m_Barrier.CreateCommandBuffer().ToConcurrent()}.Schedule(this, inputDeps);
        m_Barrier.AddJobHandleForProducer(job);
        return job;
    }

    struct SpawnPlatformsJob : IJobForEachWithEntity<SpawnPlatforms, PlatformTransforms>
    {
        [WriteOnly] public EntityCommandBuffer.Concurrent cmdBuffer;

        public void Execute(Entity entity, int index, [ReadOnly] ref SpawnPlatforms c0, [ReadOnly] ref PlatformTransforms c1)
        {
            for (var i = 0; i < c1.value.Value.translations.Length; i++)
            {
                var platform = cmdBuffer.Instantiate(index, c0.platformPrefab);
                var translation = c1.value.Value.translations[i];
                var rot = c1.value.Value.rotations[i];
                cmdBuffer.SetComponent(index, platform, new Translation{ Value = translation });
                cmdBuffer.SetComponent(index, platform, new Rotation{ Value = rot });
                cmdBuffer.AddComponent(index, platform, new PlatformId{ value = (uint)i });
                cmdBuffer.AddComponent(index, platform, new PlatformTag());
            }
            cmdBuffer.RemoveComponent(index, entity, typeof(SpawnPlatforms));
        }
    }
}