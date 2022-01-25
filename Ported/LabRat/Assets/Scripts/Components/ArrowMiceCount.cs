using Unity.Entities;
using Unity.Mathematics;

public struct ArrowMiceCount : IComponentData
{
    public int arrowUsages;

    public ArrowMiceCount(int maxUsages)
    {
        arrowUsages = maxUsages;
    }
}