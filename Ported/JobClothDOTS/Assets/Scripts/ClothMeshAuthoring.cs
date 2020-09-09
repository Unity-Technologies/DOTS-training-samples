using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public struct ClothMesh : ISharedComponentData, IEquatable<ClothMesh>
{
	public Mesh mesh;
	public NativeArray<float3> vertexPosition;
	public NativeArray<float> vertexMass;

	public bool Equals(ClothMesh other)
	{
		return (
			mesh == other.mesh &&
			vertexPosition == other.vertexPosition &&
			vertexMass == other.vertexMass
		);
	}

	public override int GetHashCode()
	{
		int hash = 0;
		{
			hash ^= ReferenceEquals(mesh, null) ? 0 : mesh.GetHashCode();
			hash ^= vertexMass.GetHashCode();
			hash ^= vertexPosition.GetHashCode();
		}
		return hash;
	}

	//TODO: tear-down
	// vertexPosition.Dispose()
	// vertexMass.Dispose()
}

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ClothMeshAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
		var mf = GetComponent<MeshFilter>();
		if (mf == null)
			return;

		var meshInstance = mf.sharedMesh;
		if (meshInstance == null)
			return;

		// initialize position
		var bufferPosition = new NativeArray<float3>(meshInstance.vertexCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
		{
			using (var meshData = Mesh.AcquireReadOnlyMeshData(meshInstance))
			{
				//NOTE: assumes that the mesh has only one group/submesh (0)
				meshData[0].GetVertices(bufferPosition.Reinterpret<Vector3>());
			}
		}

		// initialize mass
		var bufferMass = new NativeArray<float>(meshInstance.vertexCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
		{
			for (int i = 0; i != meshInstance.vertexCount; i++)
			{
				bufferMass[i] = 1.0f;
				//TODO: add pinned vertices
			}
		}

		dstManager.AddSharedComponentData(entity, new ClothMesh
		{
			mesh = meshInstance,
			vertexPosition = bufferPosition,
			vertexMass = bufferMass,
		});

		meshInstance.MarkDynamic();

		//TODO: add entities for Length
		//TODO: add entities for IndexPair
	}
}
