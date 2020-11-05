using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
unsafe public class PathDataAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField] private Color pathColour;
    [SerializeField] private RailMarkerType[] railMarkerTypes;
    [SerializeField] private int numberOfTrains = 5;
    [SerializeField] private int maxCarriages = 4;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddSharedComponentData(entity, new ID {Value = transform.GetSiblingIndex()});

        var builder = new BlobBuilder(Allocator.Temp);
        ref var pathData = ref builder.ConstructRoot<PathData>();

        var halfMarkerCount = transform.childCount;
        var totalMarkerCount = halfMarkerCount * 2;

        // Initialize Arrays
        var positionsB = builder.Allocate(ref pathData.Positions, totalMarkerCount);
        var handlesInB = builder.Allocate(ref pathData.HandlesIn, totalMarkerCount);
        var handlesOutB = builder.Allocate(ref pathData.HandlesOut, totalMarkerCount);
        var distancesB = builder.Allocate(ref pathData.Distances, totalMarkerCount);
        var markerTypesB = builder.Allocate(ref pathData.MarkerTypes, totalMarkerCount);

        var nativePositions  = positionsB.ToNativeArray();
        var nativeHandlesIn  = handlesInB.ToNativeArray();
        var nativeHandlesOut = handlesOutB.ToNativeArray();
        var nativeDistances  = distancesB.ToNativeArray();

        // Outbound Positions
        for (var c = 0; c < transform.childCount; c++)
            nativePositions[c] = transform.GetChild(c).transform.position;

        RebuildHandle(nativePositions, ref nativeHandlesIn, ref nativeHandlesOut, 0, halfMarkerCount);

        float halfDistance = BezierHelpers.MeasurePath(nativePositions, nativeHandlesIn, nativeHandlesOut, halfMarkerCount, out var tempDistances);
        for (var p = 0; p < halfMarkerCount; p++)
        {
            nativeDistances[p] = tempDistances[p];
        }

        for (int p = halfMarkerCount - 1; p >= 0; p--)
        {
            var position = nativePositions[p];
            var handleIn = nativeHandlesIn[p];
            var handleOut = nativeHandlesOut[p];

            nativePositions[totalMarkerCount - p - 1] = BezierHelpers.getOffsetPosition(position, handleIn, handleOut, Globals.BEZIER_PLATFORM_OFFSET);
        }

        RebuildHandle(nativePositions, ref nativeHandlesIn, ref nativeHandlesOut, halfMarkerCount, totalMarkerCount);

        // Marker types
        for (var m = 0; m < halfMarkerCount; m++)
        {
            // Outbound
            markerTypesB[m] = (int)railMarkerTypes[m];

            // Return
            var rmType = railMarkerTypes[halfMarkerCount - 1 - m];
            markerTypesB[halfMarkerCount + m] = (int)(rmType == RailMarkerType.ROUTE ? RailMarkerType.ROUTE
                : (rmType == RailMarkerType.PLATFORM_START ? RailMarkerType.PLATFORM_END : RailMarkerType.PLATFORM_START));
        }

        // Total path distance
        pathData.TotalDistance = BezierHelpers.MeasurePath(nativePositions, nativeHandlesIn, nativeHandlesOut, totalMarkerCount, out tempDistances);

        // Marker distances
        for (var d = 0; d < totalMarkerCount; d++)
            nativeDistances[d] = tempDistances[d];

        // Misc
        pathData.Colour = new float3(pathColour.r, pathColour.g, pathColour.b);
        pathData.NumberOfTrains = numberOfTrains;
        pathData.MaxCarriages = maxCarriages;
        
        dstManager.AddComponentData(entity, new PathDataRef
        {
            Data = builder.CreateBlobAssetReference<PathData>(Allocator.Persistent)
        });
    }


    private void RebuildHandle(NativeArray<float3> positions, ref NativeArray<float3> handlesIn, ref NativeArray<float3> handlesOut,
                               int begin, int end)
    {
        // Outbound Handles
        for (var p = begin + 1; p < end - 1; p++)
        {
            var currentPosition = positions[p];
            var previousPosition = positions[p - 1];
            var nextPosition = positions[p + 1];
            var offsetPosition = nextPosition - previousPosition;
            var handleIn = BezierHelpers.GetHandleIn(currentPosition, offsetPosition);
            var handleOut = BezierHelpers.GetHandleOut(currentPosition, offsetPosition);
            handlesIn[p] = handleIn;
            handlesOut[p] = handleOut;
        }
        // Point 0
        {
            var currentPosition = positions[begin];
            var nextPosition = positions[begin + 1];
            var offsetPosition = nextPosition - currentPosition;
            var handleIn = BezierHelpers.GetHandleIn(currentPosition, offsetPosition);
            var handleOut = BezierHelpers.GetHandleOut(currentPosition, offsetPosition);
            handlesIn[begin] = handleIn;
            handlesOut[begin] = handleOut;
        }
        // Last Point
        {
            int last = end - 1;
            var currentPosition = positions[last];
            var previousPosition = positions[last - 1];
            var offsetPosition = currentPosition - previousPosition;
            var handleIn = BezierHelpers.GetHandleIn(currentPosition, offsetPosition);
            var handleOut = BezierHelpers.GetHandleOut(currentPosition, offsetPosition);
            handlesIn[last] = handleIn;
            handlesOut[last] = handleOut;
        }
    }

    private void OnDrawGizmos()
    {
        Debug.Log("Gizmos");

        Gizmos.color = pathColour;
        for (var c = 0; c < transform.childCount; c++)
        {
            var currentPosition = transform.GetChild(c).position;
            Gizmos.DrawSphere(currentPosition, 1f);

            if (c == transform.childCount - 1)
                break;

            Gizmos.DrawLine(currentPosition, transform.GetChild(c + 1).position);
        }
    }
}
