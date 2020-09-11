#define BATCH_CREATE_EDGE_ENTITIES
#define SORT_EDGES_BY_FIRST_VERTEX// for sequential (somewhat) access in the initial configuration

using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using UnityEngine;

[Serializable]
public struct ClothMesh : ISharedComponentData, IEquatable<ClothMesh>, IDisposable
{
	public Mesh mesh;
	public NativeArray<float3> vertexPosition;
	public NativeArray<float3> vertexPositionOld;
	public NativeArray<float> vertexInvMass;
	public NativeArray<int> vertexPositionDeltaX;
	public NativeArray<int> vertexPositionDeltaY;
	public NativeArray<int> vertexPositionDeltaZ;
	public NativeArray<int> vertexPositionDeltaW;

	public void Dispose()
	{
		vertexPosition.Dispose();
		vertexPositionOld.Dispose();
		vertexInvMass.Dispose();
		vertexPositionDeltaX.Dispose();
		vertexPositionDeltaY.Dispose();
		vertexPositionDeltaZ.Dispose();
		vertexPositionDeltaW.Dispose();
	}

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

		//TODO: maybe move ClothMeshToken to chunk component data?
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
			Debug.Assert(indexCount % 3 == 0, "indexCount is not a multiple of 3");

			//TODO: parallelize
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

				// generate edge entities for all index pairs
				using (var indexPairs = edgeHashMap.GetValueArray(Allocator.TempJob))
				{
#if SORT_EDGES_BY_FIRST_VERTEX
					// sort the pairs
					indexPairs.Sort(new IndexPairComparer());
#endif

#if BATCH_CREATE_EDGE_ENTITIES
					using (var clothEdgeData = new NativeArray<ClothEdge>(indexPairs.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory))
					using (var edgeEntities = new NativeArray<Entity>(indexPairs.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory))
					{
						// create the entities
						conversionSystem.CreateAdditionalEntity(this, edgeEntities);

						// build the component data
						var clothEdgeDataJob = new CreateClothEdgeDataJob
						{
							vertexPosition = bufferPosition,
							indexPairs = indexPairs,
							clothEdgeData = clothEdgeData,
						};

						jobHandle = clothEdgeDataJob.Schedule(clothEdgeData.Length, 64);
						jobHandle.Complete();

						// add the component data to the entities
						dstManager.AddComponent<ClothEdge>(edgeEntities);
						{
							//NOTE: this query will hit only the entities that we just created
							var clothEdgeQueryDesc = new EntityQueryDesc()
							{
								All = new ComponentType[] { typeof(ClothEdge) },
								None = new ComponentType[] { typeof(ClothMesh) },
							};
							var clothEdgeQuery = dstManager.CreateEntityQuery(clothEdgeQueryDesc);

							//NOTE: this assumes that the query "hits" the underlying data in the same order as the added entities
							dstManager.AddComponentData(clothEdgeQuery, clothEdgeData);
							dstManager.AddSharedComponentData(clothEdgeQuery, clothMesh);

							// this might have been nicer?
							//dstManager.AddComponentData(edgeEntities, clothEdgeData);
							//dstManager.AddSharedComponentData(edgeEntities, clothMesh);
						}

						// and this might have been even nicer?
						//dstManager.AddComponentWithData<ClothEdge>(edgeEntities, clothEdgeData);
						//dstManager.AddSharedComponentWithData<ClothMesh>(edgeEntities, clothMesh);
					}
#else
					// loop over index pairs
					foreach (var indexPair in indexPairs)
					{
						// find the length
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
#endif
				}
			}
		}
	}

	[BurstCompile]
	public struct MassCalculationJob : IJobParallelFor
	{
		[NoAlias] public NativeArray<float> bufferInvMass;
		[NoAlias, ReadOnly] public NativeArray<float3> tempNormals;
		[NoAlias, ReadOnly] public NativeArray<float3> bufferPosition;

		public void Execute(int i)
		{
			if (tempNormals[i].y > .9f && bufferPosition[i].y > .3f)
				bufferInvMass[i] = 0.0f;
			else
				bufferInvMass[i] = 1.0f;
		}
	}

	[BurstCompile]
	public struct BufferPositionJob : IJobParallelFor
	{
		[NoAlias] public NativeArray<float3> bufferPositions;
		public Matrix4x4 localToWorld;

		public void Execute(int i)
		{
			bufferPositions[i] = localToWorld.MultiplyPoint(bufferPositions[i]);
		}
	}

	[BurstCompile]
	struct CreateClothEdgeDataJob : IJobParallelFor
	{
		[NoAlias, ReadOnly] public NativeArray<float3> vertexPosition;
		[NoAlias, ReadOnly] public NativeArray<int2> indexPairs;
		[NoAlias] public NativeArray<ClothEdge> clothEdgeData;

		public void Execute(int i)
		{
			var indexPair = indexPairs[i];
			var length = math.distance(vertexPosition[indexPair.x], vertexPosition[indexPair.y]);

			clothEdgeData[i] = new ClothEdge
			{
				IndexPair = indexPair,
				Length = length,
			};
		}
	}

	struct IndexPairComparer : IComparer<int2>
	{
		public int Compare(int2 a, int2 b)
		{
			int cx = a.x.CompareTo(b.x);
			if (cx == 0)
				return a.y.CompareTo(b.y);
			else
				return cx;
		}
	}
}
