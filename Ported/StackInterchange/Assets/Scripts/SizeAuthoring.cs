using Unity.Entities;
using Unity.Mathematics;
 
[GenerateAuthoringComponent]
public struct Size : IComponentData
{
    private char Value;

    private const int kMaxValue = 8;
    private static readonly float3[] kSize = new float3[kMaxValue]
    {
        new float3(0.4729747f, 0.2979959f, 0.9279317f),
        new float3(0.4167599f, 0.3298053f, 0.864592f),
        new float3(0.5713787f, 0.2751975f, 0.9249724f),
        new float3(0.5142478f, 0.3812511f, 0.9337433f),
        new float3(0.5129808f, 0.318708f, 0.8004383f),
        new float3(0.5637803f, 0.2006283f, 0.9411352f),
        new float3(0.4417182f, 0.2256299f, 0.9436963f),
        new float3(0.4821276f, 0.3721079f, 0.9814507f)
    };

    public void SetSize(int size)
    {
        Value = (char)(size % kMaxValue);
    }

    public float3 GetSize()
    {
        return kSize[Value];
    }
}