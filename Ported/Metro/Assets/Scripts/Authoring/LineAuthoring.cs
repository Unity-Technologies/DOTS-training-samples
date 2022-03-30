    using Unity.Entities;
using Unity.Mathematics;

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityGizmos = UnityEngine.Gizmos;
using UnityGUI = UnityEngine.GUI;
using UnityTransform = UnityEngine.Transform;
using UnityVector3 = UnityEngine.Vector3;
using UnityColor = UnityEngine.Color;

public class LineAuthoring : UnityMonoBehaviour, IConvertGameObjectToEntity
{
    public int LineID;
    [SerializeField] private int carriageCount;
    [SerializeField] private int trainCount;
    [SerializeField] private float maxSpeed;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var children = gameObject.GetComponentsInChildren<LineMarkerAuthoring>();

        dstManager.AddComponent<LineMarkerBufferElement>(entity);

        var dynamicBuffer = dstManager.GetBuffer<LineMarkerBufferElement>(entity);
        foreach (var childMarker in children)
        {
            dynamicBuffer.Add(new LineMarkerBufferElement
            {
                IsPlatform = childMarker.MarkerType == LineMarkerType.Platform,
                Position = childMarker.gameObject.transform.position
            });
        }
        
        // Bezier Curve!
        dstManager.AddComponent<BezierPointBufferElement>(entity);

        var bezierCurve = dstManager.GetBuffer<BezierPointBufferElement>(entity);
        float curveDistance = PopulateBezierFromMarkers(ref bezierCurve, children.ToList());

        dstManager.AddComponentData(entity, new LineComponent { CarriageCount = carriageCount, 
            TrainCount = trainCount,
            MaxSpeed = maxSpeed
        });
        dstManager.AddComponentData<LineTotalDistanceComponent>(entity,
            new LineTotalDistanceComponent {Value = curveDistance});
        dstManager.AddSharedComponentData<LineIDComponent>(entity, new LineIDComponent{Value = LineID});
    }

    // Populates a dynamic buffer with bezier curve information, returning the total distance.
    // An analogue of MetroLine.Create_RailPath
    public float PopulateBezierFromMarkers(ref DynamicBuffer<BezierPointBufferElement> bezierCurve,
        List<LineMarkerAuthoring> markers)
    {
        float totalPathDistance = 0;
        int totalMarkers = markers.Count;
        float3 currentLocation = float3.zero;

        // - - - - - - - - - - - - - - - - - - - - - - - -  OUTBOUND points
        for (int i = 0; i < totalMarkers; i++)
        {
            BezierHelpers.AddPoint(ref bezierCurve, markers[i].transform.position);
        }

        // fix the OUTBOUND handles
        for (int i = 0; i <= totalMarkers - 1; i++)
        {
            BezierPointBufferElement currentPoint = bezierCurve[i];
            if (i == 0)
            {
                currentPoint = BezierHelpers.SetHandles(currentPoint, bezierCurve[1].Location - currentPoint.Location);
                
            }
            else if (i == totalMarkers - 1)
            {
                currentPoint = BezierHelpers.SetHandles(currentPoint, currentPoint.Location - bezierCurve[i - 1].Location);
            }
            else
            {
                currentPoint = BezierHelpers.SetHandles(currentPoint,bezierCurve[i + 1].Location - bezierCurve[i - 1].Location);
            }

            bezierCurve[i] = currentPoint;
        }

        totalPathDistance = BezierHelpers.MeasurePath(ref bezierCurve);

        // - - - - - - - - - - - - - - - - - - - - - - - -  RETURN points
        float platformOffset = BezierHelpers.BEZIER_PLATFORM_OFFSET;
        for (int i = totalMarkers - 1; i >= 0; i--)
        {
            float3 targetLocation =
                BezierHelpers.GetPointPerpendicularOffset(bezierCurve, totalPathDistance, bezierCurve[i], platformOffset);
            BezierHelpers.AddPoint(ref bezierCurve, targetLocation);
        }
        
        // This could be off-by-one
        int returnPointOffset = totalMarkers;
        
        // fix the RETURN handles
        for (int i = 0; i <= totalMarkers - 1; i++)
        {
            BezierPointBufferElement currentReturnPoint = bezierCurve[i + returnPointOffset];
            if (i == 0)
            {
                currentReturnPoint = BezierHelpers.SetHandles(currentReturnPoint, bezierCurve[1 + returnPointOffset].Location - currentReturnPoint.Location);
            }
            else if (i == totalMarkers - 1)
            {
                currentReturnPoint = BezierHelpers.SetHandles(currentReturnPoint, currentReturnPoint.Location - bezierCurve[i - 1 + returnPointOffset].Location);
            }
            else
            {
                currentReturnPoint = BezierHelpers.SetHandles(currentReturnPoint, bezierCurve[i + 1 + returnPointOffset].Location - bezierCurve[i - 1 + returnPointOffset].Location);
            }

            bezierCurve[i + returnPointOffset] = currentReturnPoint;
        }

        totalPathDistance = BezierHelpers.MeasurePath(ref bezierCurve);
        return totalPathDistance;
    }
}
