using Unity.Entities;
using Unity.Mathematics;

public struct ChainCreateTag : IComponentData
{
}

public struct ChainStart : IComponentData
{
    public float2 Value;
}

public struct ChainEnd : IComponentData
{
    public float2 Value;
}

public struct ChainLength : IComponentData
{
    public int Value;
}

public struct ChainID : IComponentData
{
    public int Value;
}

public struct BucketInChain : IBufferElementData
{
    public int chainID;
    public int bucketPos;
    public float bucketShift;
}

public struct SharedChainComponent : ISharedComponentData
{
    public int chainID;
    public float2 start;
    public float2 end;
    public int length;
}

public struct ChainPosition : IComponentData
{
    public int Value;
}

public struct CreateChainBufferElement : IBufferElementData
{
    public int chainID;
    public int position;
    public float2 start;
    public float2 end;
    public int length;
}
