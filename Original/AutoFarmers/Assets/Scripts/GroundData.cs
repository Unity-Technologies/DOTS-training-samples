using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public struct GroundDataRegistry
{
    public BlobArray<float2> rockPositions;
    public BlobArray<float2> rockSizes;
    public BlobArray<float2> storePositions;
}

public struct GroundData : IComponentData
{
    public BlobAssetReference<GroundDataRegistry> registry;
	public Entity defaultGroundEntity;
	public Entity tilledGroundEntity;
    public MapDebugOptions debugOptions;
	public int fieldSizeX;
	public int fieldSizeY;
}