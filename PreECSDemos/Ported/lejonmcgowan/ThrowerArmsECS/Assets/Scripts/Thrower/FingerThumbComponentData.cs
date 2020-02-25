using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct  FingerThumbComponentData : IComponentData
{
    public float3 chain0; //because float3 x[4] is unsafe?
    public float3 chain1;
    public float3 chain2;
    public float3 chain3;
    
    public float3 upAxis;
    public float3 forwardAxis;
    public Translation anchor;
}
