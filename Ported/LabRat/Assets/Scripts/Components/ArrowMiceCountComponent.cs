using Unity.Entities;
using Unity.Mathematics;

public struct ArrowMiceCountComponent : IComponentData
{
    public int arrowUsages;

    public ArrowMiceCountComponent(int maxUsages)
    {
        arrowUsages = maxUsages;
    }
}