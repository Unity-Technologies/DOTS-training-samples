using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class PathDataAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField] private Color pathColour;
    [SerializeField] private RailMarkerType[] railMarkerTypes;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddSharedComponentData(entity, new ID {Value = transform.GetSiblingIndex()});

        var halfMarkerCount = transform.childCount;
        var totalMarkerCount = halfMarkerCount * 2;
        
        // Initialise Arrays
        var positions = new NativeArray<float3>(totalMarkerCount, Allocator.Temp);
        var markers = new NativeArray<int>(totalMarkerCount, Allocator.Temp);
        var handlesIn = new NativeArray<float3>(totalMarkerCount, Allocator.Temp);
        var handlesOut = new NativeArray<float3>(totalMarkerCount, Allocator.Temp);
        var distances = default(NativeArray<float>);
        var totalDistance = 0f;
        
        // Outbound Positions
        for (var c = 0; c < transform.childCount; c++)
            positions[c] = transform.position;

        // Outbound Handles
        for (var p = 0; p < halfMarkerCount; p++)
        {
            var previousPosition = positions[p == 0 ? halfMarkerCount - 1 : p - 1];
            var currentPosition = positions[p];
            var nextPosition = positions[(p + 1) % halfMarkerCount];
            
            var offsetPosition = nextPosition - previousPosition;
            var handleIn = BezierHelpers.GetHandleIn(currentPosition, offsetPosition);
            var handleOut = BezierHelpers.GetHandleOut(currentPosition, offsetPosition);
            
            handlesIn[p] = handleIn;
            handlesOut[p] = handleOut;
        }

        var halfDistance = BezierHelpers.MeasurePath(positions, handlesIn, handlesOut, out distances);
        
        // Return Positions
        for (int p = 0; p < halfMarkerCount; p++)
        {
            var position = positions[p];
            var distance = distances[p];
            var perpPosition = BezierHelpers.GetPointPerpendicularOffset(position, distance, positions, handlesIn,
                handlesOut, distances, halfDistance, Globals.BEZIER_PLATFORM_OFFSET);
            
            positions[halfMarkerCount + p] = perpPosition;
        }
        
        // Outbound Handles
        for (var p = halfMarkerCount; p < totalMarkerCount; p++)
        {
            var previousPosition = positions[p == 0 ? halfMarkerCount - 1 : p - 1];
            var currentPosition = positions[p];
            var nextPosition = positions[(p + 1) % halfMarkerCount];
            
            var offsetPosition = nextPosition - previousPosition;
            var handleIn = BezierHelpers.GetHandleIn(currentPosition, offsetPosition);
            var handleOut = BezierHelpers.GetHandleOut(currentPosition, offsetPosition);
            
            handlesIn[p] = handleIn;
            handlesOut[p] = handleOut;
        }
        
        // Marker types
        markers = new NativeArray<int>(railMarkerTypes.Select(t => (int)t).ToArray(), Allocator.Temp);
        
        // Total path distance
        totalDistance = BezierHelpers.MeasurePath(positions, handlesIn, handlesOut, out distances);

        // dstManager.AddComponentData(entity, new PathData
        // {
        //     Positions = positions,
        //     MarkerType = markers,
        //     HandlesIn = handlesIn,
        //     HandlesOut = handlesOut,
        //     Distances = distances,
        //     TotalDistance = totalDistance
        // });
    }

    private void OnDrawGizmos()
    {
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