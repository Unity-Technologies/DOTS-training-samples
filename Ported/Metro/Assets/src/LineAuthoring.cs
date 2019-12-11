using System.Collections.Generic;
using System.Linq;
using src;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
[ConverterVersion("martinsch", 8)]
public class LineAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField]
    int m_LineIndex = 0;

    public const float k_StepSize = 0.1f;
    [Range(0.001f, 0.5f)]
    public float BezierFactor = 0.5f;

    Entity m_Entity;
    EntityManager m_EntityManager;
    
    public Mesh RailMarkerMesh = null;
    public Color RailMarkerColor = Color.white;
    public Vector3 RailMarkerScale = new Vector3(0.1f, 1f, 0.025f);

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
        
        
        
        var buffer = new List<LinePositionBufferElement>();
        for (int i = 0; i < transform.childCount; i++)
        {
            buffer.Add((float3)transform.GetChild(i).position);
        } 
        m_Entity = entity;
        m_EntityManager = dstManager;
        var b = dstManager.AddBuffer<LinePositionBufferElement>(entity);
        foreach (var bufferElement in buffer)
        {
            b.Add(bufferElement);
        }
        
        var material = new Material(Shader.Find("Standard"));
        material.color = RailMarkerColor;

        for (var index = 0; index < buffer.Count; index++)
        {
            var bufferElement = buffer[index];
            var markerEntity = conversionSystem.CreateAdditionalEntity(gameObject);

            var direction = index + 1 < buffer.Count ? buffer[index + 1].Value - bufferElement.Value : bufferElement.Value - buffer[index - 1].Value;
            
            dstManager.AddComponentData(markerEntity, new Unity.Transforms.Translation()
            {
                Value = bufferElement.Value
            });
            dstManager.AddComponentData(markerEntity, new Unity.Transforms.Rotation()
            {
                Value = Quaternion.LookRotation(direction)
            });
            dstManager.AddComponentData(markerEntity, new Unity.Transforms.LocalToWorld());
            dstManager.AddComponentData(markerEntity, new Unity.Transforms.NonUniformScale()
            {
                Value = RailMarkerScale
            });
            dstManager.AddComponentData(markerEntity, new SimpleMeshRenderer
            {
                Mesh = RailMarkerMesh,
                Material = material,
            });
        }
    }
}
