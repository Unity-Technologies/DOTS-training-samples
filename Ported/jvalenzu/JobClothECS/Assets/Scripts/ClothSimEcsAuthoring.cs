using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Burst;
using Unity.Mathematics;

[RequiresEntityConversion]
[ConverterVersion("jvalenzu", 1)]
public class ClothSimEcsAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    Mesh originalMesh;

    public void OnEnable()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter)
        {
	    originalMesh = meshFilter.mesh;
	}
    }

    public void OnDestroy()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter)
        {
	    meshFilter.mesh = originalMesh;
	}
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
	if (originalMesh == null)
	{
	    originalMesh = GetComponent<MeshFilter>().mesh;
	}

	Debug.LogFormat("Convert", originalMesh == null ? "null" : "not null");
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter)
        {
            Mesh mesh = Instantiate(originalMesh);
	    meshFilter.mesh = mesh;

            DynamicBuffer<BarElement> bars = dstManager.AddBuffer<BarElement>(entity);
            bars.ResizeUninitialized(mesh.vertices.Length);
            DynamicBuffer<BarLengthElement> barLengths = dstManager.AddBuffer<BarLengthElement>(entity);
            barLengths.ResizeUninitialized(mesh.vertices.Length);
            DynamicBuffer<VertexStateCurrentElement> vertexStateCurrentElement = dstManager.AddBuffer<VertexStateCurrentElement>(entity);
            DynamicBuffer<VertexStateOldElement> vertexStateOldElement = dstManager.AddBuffer<VertexStateOldElement>(entity);

            vertexStateCurrentElement = dstManager.GetBuffer<VertexStateCurrentElement>(entity);
	    vertexStateCurrentElement.ResizeUninitialized(mesh.vertices.Length);

            vertexStateOldElement = dstManager.GetBuffer<VertexStateOldElement>(entity);
	    vertexStateOldElement.ResizeUninitialized(mesh.vertices.Length);
	    for (int i=0,n=mesh.vertices.Length; i<n; ++i)
	    {
		vertexStateCurrentElement[i] = mesh.vertices[i];
		vertexStateOldElement[i] = mesh.vertices[i];
	    }

	    ClothSimEcsSystem.AddSharedComponents(entity, originalMesh, dstManager);
        }
    }
};
