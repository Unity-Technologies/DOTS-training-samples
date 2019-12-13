using System.Collections.Generic;
using System.Linq;
using src;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
[ConverterVersion("martinsch", 10)]
public class LineAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField]
    int m_LineIndex = 0;

    [Range(0.001f, 0.5f)]
    public float BezierFactor = 0.5f;


    Entity m_Entity;
    EntityManager m_EntityManager;

    public Mesh RailMarkerMesh = null;
    public Color RailMarkerColor = Color.white;
    public Vector3 RailMarkerScale = new Vector3(0.1f, 1f, 0.025f);
    public float RailMarkerDistance = 0.1f;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Line { ID = m_LineIndex });

        var points = gameObject.GetComponentsInChildren<TrackWaypointAuthoring>().Select(wayPoint => wayPoint.transform.position).ToArray();
        var segments = new NativeArray<LineSegmentBufferElement>(points.Length, Allocator.Temp);

        for (int i = 0; i < points.Length; i++)
        {
            var pStartPrev = i > 0 ? points[i - 1] : points[points.Length - 1];
            var pStart = points[i];
            var pEnd = i + 1 < points.Length ? points[i + 1] : points[0];
            var pEndNext = i + 2 < points.Length ? points[i + 2] : points[i + 2 - points.Length];

            var startDir = Vector3.Lerp((pStart - pStartPrev).normalized, (pEnd - pStart).normalized, 0.5f);
            var endDir = Vector3.Lerp((pEnd - pEndNext).normalized, (pStart - pEnd).normalized, 0.5f);

            var controlPointDistance = Vector3.Magnitude(pEnd - pStart) * BezierFactor;

            segments[i] = new LineSegmentBufferElement()
            {
                p0 = pStart,
                p1 = pStart + startDir * controlPointDistance,
                p2 = pEnd + endDir * controlPointDistance,
                p3 = pEnd,
            };
        }
        
        dstManager.AddBuffer<LineSegmentBufferElement>(entity).CopyFrom(segments);
        
        
        
        var material = new Material(Shader.Find("Standard"));
        material.color = RailMarkerColor;

        var previousMarkerPos = float3.zero;
        foreach (var segment in segments)
        {
            CreateMarkersForSegment(segment, dstManager, conversionSystem, material, ref previousMarkerPos);
        }
    }

    private void CreateMarkersForSegment(LineSegmentBufferElement segment, EntityManager dstManager,
        GameObjectConversionSystem conversionSystem, Material material, ref float3 previousMarkerPos)
    {
        var t = 0f;
        while (t < 1.0f)
        {
            var pos = BezierMath.GetPoint(segment, t);
            if (math.length(previousMarkerPos - pos) > RailMarkerDistance)
            {
                previousMarkerPos = pos;
                var pointAhead = BezierMath.GetPoint(segment, t + 0.001f);
                CreateMarker(dstManager, conversionSystem, material, pos, math.normalize(pointAhead - pos));
            }
            
            t += 0.001f;
        }
    }

    private void CreateMarker(EntityManager dstManager,
        GameObjectConversionSystem conversionSystem, Material material, float3 translation, float3 direction)
    {
        var up = new float3(
            direction.y * direction.x / (direction.x + direction.z),
            math.length(new float2(direction.x, direction.z)),
            direction.y * direction.z / (direction.x + direction.z));
        var markerEntity = conversionSystem.CreateAdditionalEntity(gameObject);
        dstManager.AddComponentData(markerEntity, new Unity.Transforms.Translation()
        {
            Value = translation
        });
        dstManager.AddComponentData(markerEntity, new Unity.Transforms.Rotation()
        {
            Value = quaternion.LookRotation(direction, up)
        });
        dstManager.AddComponentData(markerEntity, new Unity.Transforms.LocalToWorld());
//        dstManager.AddComponentData(markerEntity, new Unity.Transforms.NonUniformScale()
//        {
//            Value = RailMarkerScale
//        });
        dstManager.AddSharedComponentData(markerEntity, new RenderMesh
        {
            mesh = RailMarkerMesh,
            material = material,
        });
    }
}
