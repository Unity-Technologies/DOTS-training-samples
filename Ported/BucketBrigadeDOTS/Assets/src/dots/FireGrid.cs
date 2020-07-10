using Unity.Entities;
using Unity.Mathematics;

// Fire simulation grid settings

[GenerateAuthoringComponent]
public struct FireGridSettings : IComponentData
{
	// The resolution of the grid
    public uint2 FireGridResolution;
    // Current mip to visualize (-1 -> numMips -1)
    public int MipDebugIndex;
    public float FlashPoint;
    public float HeatTransferRate;
    public uint ExtinguishRadius;
    public float CoolingStrength;
    public float CoolingStrenghtFalloff;
    public uint BucketCapacity;
}

public struct FireCell : IBufferElementData
{
    public float FireTemperature;
}

// Not as bad as we thought
public struct FireCellHistory : IBufferElementData
{
    public float FireTemperaturePrev;
}

public struct FireCellFlag : IBufferElementData
{
    public bool OnFire;
}

public struct FireGridMipLevelData : IBufferElementData
{
	// Resolution of the mip level
    public uint2 dimensions;
    // Offset of the mip data in the buffer
    public uint offset;
    // World space cell size
    public float2 cellSize;
    // The position of the center of 
    // bottom left corner of the grid
    public float2 minCellPosition;
}


