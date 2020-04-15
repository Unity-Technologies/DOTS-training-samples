

using Unity.Entities;
using Unity.Mathematics;

public struct ResourceSourcePosition : IComponentData
{
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
public struct BrigadeLine : IComponentData
{
}

// This goes on the worker
public struct BrigadeLineRef : ISharedComponentData
{
    public Entity BrigadeLineEntity;
    public override int GetHashCode() => BrigadeLineEntity.GetHashCode();
    public override bool Equals(object obj) => ((BrigadeLineRef)obj).BrigadeLineEntity.Equals(BrigadeLineEntity);
}

public struct ResourceQuantity : IComponentData
{
    public int Value;
}



public struct BrigadeInitInfo : IComponentData
{
    public int WorkerCount;
}

public struct WorkerPositionsNeedUpdate : IComponentData
{
}
/*
//Create Line
Entities
    .WithAll<BrigadeInitInfo>() //created during conversion
	.WithNone<BrigadeLine>()
=>add BrigadeLine components

//Find Resource System
Entities
    .WithAll<BrigadeLine>()
	.WithNone<ResourceSourcePosition>()

//Find Target System
Entities
   .WithAll<BrigadeLine>()
	.WithNone<ResourceTargetPosition>()

//Update worker positions. Query all lines that need updating. Find their workers using new BrigadeLineRef(lineEntity).
Entities
    .WithAll<WorkerPositionsNeedUpdate>()


    */
