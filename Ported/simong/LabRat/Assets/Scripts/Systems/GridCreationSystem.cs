using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class GridCreationSystem : SystemBase
{
    public NativeArray<CellInfo> Cells { get; private set; }

    protected override void OnCreate()
    {
        var constantData = ConstantData.Instance;
        Cells = new NativeArray<CellInfo>(constantData.BoardDimensions.x * constantData.BoardDimensions.y, Allocator.Persistent);

        base.OnCreate();
    }

    protected override void OnDestroy()
    {
        if (Cells.IsCreated)
        {
            Cells.Dispose();
        }

        base.OnDestroy();
    }

    protected override void OnUpdate()
    {

    }
}
