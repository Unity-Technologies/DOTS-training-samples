using Unity.Collections;
using Unity.Mathematics;

public struct PathDataNative
{
    public readonly NativeArray<float3> Positions;
    public readonly NativeArray<float3> HandlesIn;
    public readonly NativeArray<float3> HandlesOut;
    public readonly NativeArray<float> Distances;
    public readonly NativeArray<int> MarkerTypes;
    public readonly float TotalDistance;
    public readonly float3 Colour;
    public readonly int NumberOfTrains;
    public readonly int CarriagesPerTrain;

    public PathDataNative(NativeArray<float3> positions, NativeArray<float3> handlesIn, NativeArray<float3> handlesOut, NativeArray<float> distances, NativeArray<int> markerTypes, float totalDistance, float3 colour, int numberOfTrains, int carriagesPerTrain)
    {
        Positions = positions;
        HandlesIn = handlesIn;
        HandlesOut = handlesOut;
        Distances = distances;
        MarkerTypes = markerTypes;
        TotalDistance = totalDistance;
        Colour = colour;
        NumberOfTrains = numberOfTrains;
        CarriagesPerTrain = carriagesPerTrain;
    }
}