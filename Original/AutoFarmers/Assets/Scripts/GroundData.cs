using UnityEngine;
using Unity.Entities;

public struct GroundDataRegistry
{
    // TODO: arrays?
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