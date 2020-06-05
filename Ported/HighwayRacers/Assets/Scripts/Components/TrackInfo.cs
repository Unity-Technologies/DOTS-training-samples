using System;
using Unity.Entities;
using Unity.Collections;

public class TrackInfo : IComponentData
{
    //TODO: implement functions correctly
    public NativeArray<float> Progresses;
    public NativeArray<float> Speeds;
    public NativeArray<float> Lanes;
}
