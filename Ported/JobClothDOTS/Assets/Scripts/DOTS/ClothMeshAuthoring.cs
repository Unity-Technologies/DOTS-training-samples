using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using System.Diagnostics;

[Serializable]
public struct ClothMesh : ISharedComponentData, IEquatable<ClothMesh>
{
	public Mesh mesh;
	public NativeArray<float3> vertexPosition;
	public NativeArray<float3> vertexPositionOld;
	public NativeArray<float> vertexInvMass;
	public NativeArray<int> vertexPositionDeltaX;
	public NativeArray<int> vertexPositionDeltaY;
	public NativeArray<int> vertexPositionDeltaZ;
	public NativeArray<int> vertexPositionDeltaW;

	public bool Equals(ClothMesh other)
	{
		return (
			mesh == other.mesh &&
			vertexPosition == other.vertexPosition &&
			vertexPositionOld == other.vertexPositionOld &&
			vertexInvMass == other.vertexInvMass &&
			vertexPositionDeltaX == other.vertexPositionDeltaX &&
			vertexPositionDeltaY == other.vertexPositionDeltaY &&
			vertexPositionDeltaZ == other.vertexPositionDeltaZ &&
			vertexPositionDeltaW == other.vertexPositionDeltaW &&
			true// end
		);
	}

	public override int GetHashCode()
	{
		int hash = 0;
		{
			hash ^= ReferenceEquals(mesh, null) ? 0 : mesh.GetHashCode();
			hash ^= vertexPosition.GetHashCode();
			hash ^= vertexPositionOld.GetHashCode();
			hash ^= vertexInvMass.GetHashCode();
			hash ^= vertexPositionDeltaX.GetHashCode();
			hash ^= vertexPositionDeltaY.GetHashCode();
			hash ^= vertexPositionDeltaZ.GetHashCode();
			hash ^= vertexPositionDeltaW.GetHashCode();
		}
		return hash;
	}

	//TODO: tear-down
	// vertexPosition.Dispose()
	// vertexInvMass.Dispose()
}

public struct MassCalculationJob : IJobParallelFor
{
	public NativeArray<float> bufferInvMass;
	public NativeArray<float3> tempNormals;
	public NativeArray<float3> bufferPosition;

	public void Execute(int i)
    {
		if (tempNormals[i].y > .9f && bufferPosition[i].y > .3f)
			bufferInvMass[i] = 0.0f;
		else
			bufferInvMass[i] = 1.0f;
	}
}

public struct BufferPositionJob : IJobParallelFor
{
	public Matrix4x4 localToWorld;
	public NativeArray<float3> bufferPositions;

	public void Execute(int i) 
	{
		bufferPositions[i] = localToWorld.MultiplyPoint(bufferPositions[i]);
	}
}

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ClothMeshAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
	public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
		JobHandle jobHandle;

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
		var bufferInvMass = new NativeArray<float>(meshInstance.vertexCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
		{
			using (var meshData = Mesh.AcquireReadOnlyMeshData(meshInstance))
			using (var tempNormals = new NativeArray<float3>(meshInstance.vertexCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory))
			{
				meshData[0].GetNormals(tempNormals.Reinterpret<Vector3>());

				var massJob = new MassCalculationJob
				{
					bufferInvMass = bufferInvMass,
					tempNormals = tempNormals,
					bufferPosition = bufferPosition
				};

				jobHandle = massJob.Schedule(meshInstance.vertexCount, 64);

				jobHandle.Complete();
			}
		}

		// transform positions to world space
		var localToWorld = this.transform.localToWorldMatrix;

		var bufferPosJob = new BufferPositionJob
		{
			localToWorld = localToWorld,
			bufferPositions = bufferPosition
		};

		jobHandle = bufferPosJob.Schedule(meshInstance.vertexCount, 64);

		jobHandle.Complete();

		// create the shared data
		var clothMesh = new ClothMesh
		{
			mesh = meshInstance,
			vertexPosition = bufferPosition,
			vertexPositionOld = new NativeArray<float3>(bufferPosition, Allocator.Persistent),
			vertexInvMass = bufferInvMass,
			vertexPositionDeltaX = new NativeArray<int>(meshInstance.vertexCount, Allocator.Persistent),
			vertexPositionDeltaY = new NativeArray<int>(meshInstance.vertexCount, Allocator.Persistent),
			vertexPositionDeltaZ = new NativeArray<int>(meshInstance.vertexCount, Allocator.Persistent),
			vertexPositionDeltaW = new NativeArray<int>(meshInstance.vertexCount, Allocator.Persistent),
		};

		// add shared data to entity
		dstManager.AddSharedComponentData(entity, clothMesh);
		dstManager.AddComponentData(entity, new ClothMeshToken { jobHandle = new JobHandle() });
		//dstManager.AddChunkComponentData<ClothMeshToken>(entity);

		// spawn entities for the edges
		using (var meshData = Mesh.AcquireReadOnlyMeshData(meshInstance))
		{
			// finding unique edges in mesh:
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
			//Debug.Assert(indexCount % 3 == 0, "indexCount is not a multiple of 3");

			using (var edgeHashMap = new NativeHashMap<ulong, int2>(indexCount, Allocator.Temp))
			{

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
						if (bufferInvMass[v0] + bufferInvMass[v1] == 0.0f)
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

					//TODO: speed up
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
}
