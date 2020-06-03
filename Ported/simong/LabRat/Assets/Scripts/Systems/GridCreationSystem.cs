using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


[ExecuteAlways]
public class GridCreationSystem : SystemBase
{
    public NativeArray<CellInfo> Cells { get; private set; }

    protected override void OnCreate()
    {
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
        var constantData = ConstantData.Instance;

        if(!Cells.IsCreated && constantData != null)
        {
            // Ugly Simon's code, if it works, don't change it!
            unsafe
            {
                Unity.Physics.BoxCollider* boardCollider = (Unity.Physics.BoxCollider*)GetSingleton<Unity.Physics.PhysicsCollider>().ColliderPtr;
                var geometry = boardCollider->Geometry;
                geometry.Size = new float3(constantData.BoardDimensions.x, 1, constantData.BoardDimensions.y);
                geometry.Center = new float3(constantData.BoardDimensions.x / 2, -1, constantData.BoardDimensions.y / 2);
                boardCollider->Geometry = geometry;
            }

            Cells = new NativeArray<CellInfo>(constantData.BoardDimensions.x * constantData.BoardDimensions.y, Allocator.Persistent);
            Debug.Log("cell infos created");
        }
    }
}
