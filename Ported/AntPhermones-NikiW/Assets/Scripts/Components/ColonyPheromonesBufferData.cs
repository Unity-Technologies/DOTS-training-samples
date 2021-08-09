using System;
using Unity.Entities;
using Unity.NetCode;

/// <summary>
///     We need the IBufferElementData to ensure the NetCode can serialize.
///     It's not the most efficient method, but it works!
///     InternalBufferCapacity set to max out chunk memory, as singleton.
/// </summary>
[GenerateAuthoringComponent]
[InternalBufferCapacity(2000)]
public struct ColonyPheromonesBufferData : IBufferElementData
{
    [GhostField(Quantization = 1000)]
    public float Value;
}