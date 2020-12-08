using Unity.Collections;
using Unity.Entities;

public struct Building : IComponentData
{
    public NativeArray<Constraint> Constraints;
}