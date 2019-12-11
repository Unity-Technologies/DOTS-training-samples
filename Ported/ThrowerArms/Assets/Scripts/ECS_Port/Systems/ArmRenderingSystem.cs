using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public class ArmRenderingSystem : JobComponentSystem
{ 
    private EntityQuery _antQuery;
    private EntityQuery _armSharedRenderingQuery;
    private bool _allocatedBatchData;

    private readonly List<Matrix4x4[]> _matrices = new List<Matrix4x4[]>();
    private readonly List<Vector4[]> _colours = new List<Vector4[]>();
    private readonly List<GCHandle> _gcHandlesToFree = new List<GCHandle>();
    private readonly List<MaterialPropertyBlock> _materialPropertyBlocks = new List<MaterialPropertyBlock>();

    protected override void OnCreate()
    {
        base.OnCreate();
        
        
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        throw new NotImplementedException();
    }
}
