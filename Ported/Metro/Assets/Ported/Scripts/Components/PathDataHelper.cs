public static class PathDataHelper
{
    public static PathDataNative ToNativePathData(this in PathDataRef pathDataRef)
    {
        return new PathDataNative(
            pathDataRef.Data.Value.Positions.ToNativeArray(),
            pathDataRef.Data.Value.HandlesIn.ToNativeArray(),
            pathDataRef.Data.Value.HandlesOut.ToNativeArray(),
            pathDataRef.Data.Value.Distances.ToNativeArray(),
            pathDataRef.Data.Value.MarkerTypes.ToNativeArray(),
            pathDataRef.Data.Value.TotalDistance,
            pathDataRef.Data.Value.Colour,
            pathDataRef.Data.Value.NumberOfTrains,
            pathDataRef.Data.Value.MaxCarriages
        );
    }
}