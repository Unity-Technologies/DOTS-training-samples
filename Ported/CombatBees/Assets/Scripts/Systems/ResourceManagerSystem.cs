using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using Unity.Transforms;

public class ResourceManagerSystem : SystemBase
{
    ResourceParams resParams;
    ResourceGridParams resGridParams;
    DynamicBuffer<StackHeightParams> stackHeights;

    protected override void OnCreate()
    {
        resParams = GetSingleton<ResourceParams>();
        resGridParams = GetSingleton<ResourceGridParams>();
        var bufferFromEntity = GetBufferFromEntity<StackHeightParams>();
        var bufferEntity = GetSingletonEntity<ResourceParams>();
        stackHeights = bufferFromEntity[bufferEntity];

    }
    protected override void OnUpdate()
    {
       
    }



}