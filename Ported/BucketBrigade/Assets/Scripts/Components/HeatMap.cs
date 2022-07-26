using Unity.Entities;
using Unity.Collections;

// An empty component is called a "tag component".
struct HeatMap : IComponentData
{
    public NativeArray<float> HeatValues;
}