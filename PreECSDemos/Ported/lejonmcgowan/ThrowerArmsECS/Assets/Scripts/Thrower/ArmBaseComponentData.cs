using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct ArmBaseComponentData: IComponentData
{
    public Translation target;
    public Translation anchorPosition;
    public float3 anchorRight;
    public float3 LastHandUp;
    public float grabT;
    public float reach;
}