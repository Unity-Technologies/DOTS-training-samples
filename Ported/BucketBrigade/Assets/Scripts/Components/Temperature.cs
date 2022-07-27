using Unity.Entities;

[BakingType]
struct Temperature : IBufferElementData
{
    public float Value;
}