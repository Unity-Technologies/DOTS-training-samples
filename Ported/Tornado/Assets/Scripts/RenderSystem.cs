using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public class PushToRenderSystem : JobComponentSystem
{
    EntityQuery generationSettings;
    const int instancesPerBatch = 1023;
    Matrix4x4[] arraySlice;
//    Matrix4x4[][] batches;

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<GenerationSystem.State>();
        generationSettings = GetEntityQuery(typeof(GenerationSetting));
        arraySlice = new Matrix4x4[instancesPerBatch];
    }
    
//    private EntityQuery settingsQuery;
//
//    protected override void OnCreate()
//    {
//        base.OnCreate();
//
//        settingsQuery = GetEntityQuery(new EntityQueryDesc {All = new ComponentType[] {typeof(GenerationSetting)}});
//        RequireForUpdate(settingsQuery);
//    }
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var settingsEntity = generationSettings.GetSingletonEntity();
        var settings = EntityManager.GetComponentObject<GenerationSetting>(settingsEntity);
        
        Entities.WithoutBurst().ForEach( (Entity entity, ref DynamicBuffer<RenderMatrixEntry> matricies) =>
        {
//            var matrixArray = matricies.AsNativeArray().Reinterpret<Matrix4x4>();
//            var numBatches = matrixArray.Length / instancesPerBatch + 1;
//            // TODO: Initialize batches supporting a number of Matrecies higher than instancesPerBatch

//            var arraySlice = matrixArray.GetSubArray(0, 700).ToArray();
//            for (int i = 0; i < numBatches; i++)
//                Graphics.DrawMeshInstanced(settings.barMesh, 0, settings.barMaterial, arraySlice, 700);
//                Graphics.DrawMeshInstanced(settings.barMesh, 0, settings.barMaterial, batches[i], batches[i].Length);
//
            for (int i = 0; i < matricies.Length; i++)
                Graphics.DrawMesh(settings.barMesh, matricies[i].Value, settings.barMaterial, 0);

        }).Run();
        
        return inputDeps;
    }
}