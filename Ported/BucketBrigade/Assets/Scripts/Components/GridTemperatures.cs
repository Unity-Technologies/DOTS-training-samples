using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct GridTemperatures : IComponentData
{
    public double NextGridUpdateTime;
    NativeArray<float> temperatures;
    int sqrSize;
    bool isInit;

    public void Init(int sideSize)
    {
        temperatures = new NativeArray<float>(sideSize * sideSize, Allocator.Persistent);
        sqrSize = sideSize;
        isInit = true;
    }

    public float Get(int x, int y)
    {
        if (!isInit) throw new Exception("not initialized");
        // 10 x 10: [0,0]==[0]; [5, 0]==[5+0]==[5]; [5, 5]==[5+5*10]==[55]; [9,9]==[9+9*10]==99; [10,9]==[10+9*10]==[100]==error; [9,10]==[9+10*10]==[109]==error
        return temperatures[x + y * sqrSize];
    }

    public void Set(int x, int y, float value)
    {
        if (!isInit) throw new Exception("not initialized");
        temperatures[x + y * sqrSize] = value;
    }
}
