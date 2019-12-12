using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public class PushToRenderSystem : JobComponentSystem
{
    EntityQuery generationSettings;
    EntityQuery particleSettings;
    const int instancesPerBatch = 1023;
    Matrix4x4[] instances;
//    Matrix4x4[][] batches;

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<GenerationSystem.State>();
        generationSettings = GetEntityQuery(typeof(GenerationSetting));
        particleSettings = GetEntityQuery(typeof(ParticleSettings));
        instances = new Matrix4x4[instancesPerBatch];
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var settingsEntity = generationSettings.GetSingletonEntity();
        var settings = EntityManager.GetComponentObject<GenerationSetting>(settingsEntity);
        
        Entities.WithoutBurst().ForEach( (Entity entity, in DynamicBuffer<RenderMatrixEntry> matricies) =>
        {
            var numBatches = matricies.Length / instancesPerBatch + 1;
            for (int i = 0; i < numBatches; i++)
            {
                var start = i * instancesPerBatch;
                var length = math.min(instancesPerBatch, matricies.Length - start);
                NativeArray<Matrix4x4>.Copy(matricies.AsNativeArray().Reinterpret<Matrix4x4>(), start, instances, 0, length);
                Graphics.DrawMeshInstanced(settings.barMesh, 0, settings.barMaterial, instances, length);
            }
        }).Run();
        
        return inputDeps;
    }
}