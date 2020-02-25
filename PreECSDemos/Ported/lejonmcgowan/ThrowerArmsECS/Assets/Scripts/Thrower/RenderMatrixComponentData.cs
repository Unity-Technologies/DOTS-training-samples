using Unity.Entities;
using Unity.Mathematics;

public struct RenderMatrixComponentData: IComponentData
{
    public float4x4 matrix;
}