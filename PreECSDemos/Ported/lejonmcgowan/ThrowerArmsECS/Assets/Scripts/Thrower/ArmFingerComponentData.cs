using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct ArmFingerComponentData: IComponentData
{
    public float3 position;
    public float3 forward;
    public float3 anchorUp;
}
