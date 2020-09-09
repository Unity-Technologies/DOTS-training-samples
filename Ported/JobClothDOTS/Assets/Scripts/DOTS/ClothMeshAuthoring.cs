using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[Serializable]
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
	NativeArray<Vector3> vertices;
	public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
		var mf = GetComponent<MeshFilter>();
		if (mf == null)
			return;

		var meshInstance = mf.sharedMesh;
		if (meshInstance == null)
			return;

		meshInstance = Mesh.Instantiate(meshInstance);
		meshInstance.name += " (instance)";

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
			vertices = new NativeArray<Vector3>(meshInstance.vertices,Allocator.Persistent);
			Vector3[] normals = meshInstance.normals;
			for (int i = 0; i != meshInstance.vertexCount; i++)
			{
				if (normals[i].y>.9f && vertices[i].y>.3f) 
					bufferMass[i] = 0f;
				else
					bufferMass[i] = 1.0f;
				//TODO: add pinned vertices
			}
		}

		// create the shared data
		var clothMesh = new ClothMesh
		{
			mesh = meshInstance,
			vertexPosition = bufferPosition,
			vertexMass = bufferMass,
		};

		// add shared data to entity
		dstManager.AddSharedComponentData(entity, clothMesh);
		dstManager.AddComponentData(entity, new ClothMeshToken { jobHandle = new JobHandle() });

		// spawn entities for the edges
		using (var meshData = Mesh.AcquireReadOnlyMeshData(meshInstance))
		{
			// finding edges in mesh:
			//
			// 1. pull the triangle indices
			// 2. loop over triangles
			//    a. loop over edges in each triangle
			//       i. sort the vertex indices
			//       ii. make 64 bit key based on sorted vertex indices
			//       iii. use 64 bit key to update NativeHashMap
			// 3. the NativeHashMap now contains all unique index pairs
			// 4. loop over index pairs
			//    a. find the length
			//    b. create an entity with the ClothMesh and ClothEdge components

			int indexCount = meshData[0].GetSubMesh(0).indexCount;
			var indexBuffer = new NativeArray<int>(indexCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

			// pull the triangle indices
			meshData[0].GetIndices(indexBuffer, 0);
			Debug.Assert(indexCount % 3 == 0, "indexCount is not a multiple of 3");

			var edgeHashMap = new NativeHashMap<ulong, int2>(indexCount, Allocator.Temp);

			// loop over triangles
			for (int i = 0; i != indexCount; i += 3)
			{
				// triangle edges are
				// i,j
				// j,k
				// k,i

				int j = i + 1;
				int k = i + 2;

				// loop over edges in each triangle
				for (int e = 0; e != 3; e++)
				{
					int v0 = indexBuffer[e == 2 ? k : i + e];
					int v1 = indexBuffer[e == 2 ? i : j + e];

					// sort the vertex indices
					if (v0 > v1)
					{
						var tmp = v0;
						v0 = v1;
						v1 = tmp;
					}

					// skip edge if both vertices have zero mass (pinned)
					if (bufferMass[v0] + bufferMass[v1] == 0.0f)
						continue;

					// make 64 bit key based on sorted vertex indices
					var key = (ulong)((uint)v1) << 32 | (ulong)((uint)v0);

					// use 64 bit key to update NativeHashMap
					var indexPair = new int2
					{
						x = v0,
						y = v1,
					};

					edgeHashMap.TryAdd(key, indexPair);
				}
			}

			// loop over index pairs
			foreach (var edgeKeyValue in edgeHashMap)
			{
				// find the length
				var indexPair = edgeKeyValue.Value;
				var length = math.distance(bufferPosition[indexPair.x], bufferPosition[indexPair.y]);

				// create an entity with the ClothMesh and ClothEdge components
				var edgeEntity = conversionSystem.CreateAdditionalEntity(this);
				{
					dstManager.AddSharedComponentData(edgeEntity, clothMesh);
					dstManager.AddComponentData(edgeEntity, new ClothEdge
					{
						IndexPair = indexPair,
						Length = length,
					});
				}
			}
		}
	}
}
