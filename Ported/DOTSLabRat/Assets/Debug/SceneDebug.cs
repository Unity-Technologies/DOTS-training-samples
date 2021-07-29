using System;
using DOTSRATS;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class SceneDebug : MonoBehaviour
{
    public NativeArray<CellStruct> CellStructs { private set; get; }
    public BoardSpawner BoardSpawner;
    public GameState GameState;

    public void SetCellStructs(DynamicBuffer<CellStruct> cellStructs)
    {
        if (CellStructs.IsCreated)
            CellStructs.Dispose();
        CellStructs = cellStructs.ToNativeArray(Allocator.Persistent);
    }

    public void OnDisable()
    {
        if (CellStructs.IsCreated)
            CellStructs.Dispose();
    }
}
