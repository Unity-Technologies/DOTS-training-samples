using System;
using Unity.Collections;
using Unity.Entities;

// ReSharper disable once InconsistentNaming
public struct GridComponent : IComponentData
{
    /* Grid Values */
    // Negative Values - rock offset
    
    // 1...[RockOffsetX][RockOffsetY]
    
    // 0 - nothing
    // 1 - Tilled
    // 2 - Shop
    // 3 - Plant
    // 4 - Rock
    // Larger Odd Numbers health of Plant
    // Larger Even Numbers health or Rock

    public unsafe fixed int tiles[8*8];
    public int Size;
}
