using Unity.Entities;
using Unity.Mathematics;

public struct PathData : IComponentData
{
    public BlobArray<float3> Positions;
    public BlobArray<float3> HandlesIn;
    public BlobArray<float3> HandlesOut;
    public BlobArray<float> Distances;
    public BlobArray<int> MarkerTypes;
    public float TotalDistance;
    public float3 Colour;
    public int NumberOfTrains;
    public int MaxCarriages;
}