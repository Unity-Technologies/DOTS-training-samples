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
        
        var builder = new BlobBuilder(Allocator.Temp);
        ref var pathData = ref builder.ConstructRoot<PathData>();

        var halfMarkerCount = transform.childCount;
        var totalMarkerCount = halfMarkerCount * 2;
        
        // Initialise Arrays
        var positions = builder.Allocate(ref pathData.Positions, totalMarkerCount);
        var markerTypes = builder.Allocate(ref pathData.MarkerTypes, totalMarkerCount);
        var handlesIn = builder.Allocate(ref pathData.HandlesIn, totalMarkerCount);
        var handlesOut = builder.Allocate(ref pathData.HandlesOut, totalMarkerCount);
        var distances = builder.Allocate(ref pathData.Distances, totalMarkerCount);
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

        var halfDistance = BezierHelpers.MeasurePath(positions, handlesIn, handlesOut, out var tempDistances);
        
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
        for (var m = 0; m < halfMarkerCount; m++)
        {
            // Outbound
            markerTypes[m] = (int) railMarkerTypes[m];
            
            // Return
            markerTypes[halfMarkerCount + m] = (int)railMarkerTypes[halfMarkerCount - 1 - m];
        }

        // Total path distance
        totalDistance = BezierHelpers.MeasurePath(positions, handlesIn, handlesOut, out tempDistances);
        pathData.TotalDistance = totalDistance;
        
        // Marker distances
        for (var d = 0; d < totalMarkerCount; d++)
            distances[d] = tempDistances[d];
        
        // Path colour
        pathData.Colour = new float3(pathColour.r, pathColour.g, pathColour.b);

        dstManager.AddComponentData(entity, new PathRef
        {
            Data = builder.CreateBlobAssetReference<PathData>(Allocator.Persistent)
        });
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