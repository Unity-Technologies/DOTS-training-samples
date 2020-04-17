

using Unity.Entities;
using Unity.Mathematics;

public struct ResourceSourcePosition : IComponentData
{
    public Entity Id;
    public float2 Value;
}

public struct ResourceTargetPosition : IComponentData
{
    public float2 Value;
}
/*
Entities.Foreach(Entity e, in ref BrigadeLine line, ref Worker worker)
{
    ///e is the worker,
}*/

// This goes on the brigade entity

/*
// This goes on the worker
public struct BrigadeLineRef : ISharedComponentData
{
    public Entity BrigadeLineEntity;
    public override int GetHashCode() => BrigadeLineEntity.GetHashCode();
    public override bool Equals(object obj) => ((BrigadeLineRef)obj).BrigadeLineEntity.Equals(BrigadeLineEntity);
}
*/
public struct ResourceQuantity : IComponentData
{
    public int Value;
}

public struct FireStartGridPosition : IComponentData
{
    public int2 Value;
}
