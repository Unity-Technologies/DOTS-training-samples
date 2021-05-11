using Unity.Collections;
using Unity.Entities;

struct OffsetList : IComponentData
{
    NativeArray<float> Values;
}