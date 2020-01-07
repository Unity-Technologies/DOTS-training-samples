using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using Unity.Rendering;

public class ClothSimEcsSystem : JobComponentSystem
{
    static Dictionary<Mesh, ClothBarSimEcs> s_MeshToBarSimLookup = new Dictionary<Mesh, ClothBarSimEcs>();
    NativeArray<JobHandle> jobHandles;
    EntityQuery simGroup;

    // arbitrary estimate of initial entities
    enum EntityHighwater
    {
        kInitial = 5
    };

    // we both find relevant entities and partition their immutable data via the ClothBarSimEcs
    // component.
    //
    // debated moving this into a two-stage initialization, but it's tough to actually jobify since
    // we're going to end up accessing the managed Mesh type for vertex and normal data.  Moving
    // it the other direction, to a tool time conversion, seems preferable.
    static public void AddSharedComponents(Entity entity, Mesh mesh, EntityManager dstManager)
    {
        if (!s_MeshToBarSimLookup.ContainsKey(mesh))
        {
            NativeArray<byte> pins = new NativeArray<byte>(mesh.vertices.Length, Allocator.Persistent);

            if (mesh.normals == null)
            {
                for (int i=0,n=pins.Length; i<n; ++i) {
                    if (mesh.vertices[i].y > 0.3f)
                        pins[i] = 1;
                }
            }
            else
            {
                for (int i=0,n=pins.Length; i<n; ++i) {
                    if (mesh.normals[i].y >= 0.9f && mesh.vertices[i].y > 0.3f)
                        pins[i] = 1;
                }
            }
            
            HashSet<Vector2Int> barLookup = new HashSet<Vector2Int>();
            int[] triangles = mesh.triangles;
            for (int i=0, n=triangles.Length; i<n; i += 3)
            {
                for (int j=0; j<3; ++j)
                {
                    Vector2Int pair = new Vector2Int();
                    pair.x = triangles[i + j];
                    pair.y = triangles[i + (j + 1)%3];
                    if (pair.x > pair.y) {
                        int temp = pair.x;
                        pair.x = pair.y;
                        pair.y = temp;
                    }

                    if (barLookup.Contains(pair) == false &&
                        // two pinned verts can't move, so don't simulate them
                        pins[pair.x] + pins[pair.y] != 2)
                    {
                        barLookup.Add(pair);
                    }
                }
            }
            List<Vector2Int> barList = new List<Vector2Int>(barLookup);
            NativeArray<ClothConstraint> constraints = new NativeArray<ClothConstraint>(barList.Count, Allocator.Persistent);

            for (int i=0,n=barList.Count; i<n; ++i) {
                Vector2Int pair = barList[i];
                Vector3 p1 = mesh.vertices[pair.x];
                Vector3 p2 = mesh.vertices[pair.y];

                ClothConstraint constraint = new ClothConstraint();
                constraint.x = (ushort) pair.x;
                constraint.y = (ushort) pair.y;
                constraint.pinPair = (ushort) ((pins[pair.x]<<1) | pins[pair.y]);
                constraint.length = (ushort) ((p2 - p1).magnitude * 256);

                constraints[i] = constraint;
            }

            ClothBarSimEcs barSimEcs = new ClothBarSimEcs();
            barSimEcs.pinState = pins;
            barSimEcs.constraints = constraints;

            s_MeshToBarSimLookup.Add(mesh, barSimEcs);
        }
        
        dstManager.AddSharedComponentData(entity, s_MeshToBarSimLookup[mesh]);
    }

    [BurstCompile]
    struct ClothBarSimJob : IJob {
        [NativeDisableContainerSafetyRestriction]
        public NativeArray<float3> vertices;
        [ReadOnly]
        public NativeArray<ClothConstraint> constraints;

        // p0 | p1 | c0 | d0
        // -----------------
        //  0 |  0 |  1 |  1
        //  0 |  1 |  2 |  0
        //  1 |  0 |  0 |  2
        //  1 |  1 |  0 |  0
        readonly static float[] scaleX = {
            1.0f,
            2.0f,
            0.0f,
            0.0f
        };
        readonly static float[] scaleY = {
            1.0f,
            0.0f,
            2.0f,
            0.0f
        };

        public void Execute() {
            for (int i=0,n=constraints.Length; i<n; ++i)
            {
                ClothConstraint constraint = constraints[i];

                float length = constraint.length * 1.953125e-3f; // (1/256) * (1/2)
                float3 p1 = vertices[constraint.x];
                float3 p2 = vertices[constraint.y];
                float3 v0 = p2 - p1;
                float3 v1 = v0 * (0.5f - length / math.length(v0));

                int pinPairIndex = constraint.pinPair;
                p1 += v1 * scaleX[pinPairIndex];
                p2 -= v1 * scaleY[pinPairIndex];

                vertices[constraint.x] = p1;
                vertices[constraint.y] = p2;
            }
        }
    }

    [BurstCompile]
    struct ClothSimVertexJob0 : IJobParallelOverVertices {
        [ReadOnly]
        public float4x4 localToWorld;
        [ReadOnly]
        public float4x4 worldToLocal;
        [ReadOnly]
        public float3 gravity;
        [ReadOnly]
        public float localY0;
        [ReadOnly]
        public NativeArray<byte> pins;

        [NativeDisableContainerSafetyRestriction]
        public NativeArray<float3> currentVertexState;
        [NativeDisableContainerSafetyRestriction]
        public NativeArray<float3> oldVertexState;

        public void Execute(int startIndex, int count) {
            for (int i=startIndex,n=startIndex+count; i<n; ++i)
            {
                if (pins[i] != 0)
                    continue;

                float3 oldVert = oldVertexState[i];
                float3 vert = currentVertexState[i];

                float3 v0 = vert;
                float3 v1 = 2.0f * vert - oldVert + gravity;

                if (v1.y < localY0) {
                    float3 worldPos = math.transform(localToWorld, v1);
                    Vector3 oldWorldPos = math.transform(localToWorld, v0);
                    oldWorldPos.y = (worldPos.y - oldWorldPos.y) * .5f;
                    worldPos.y = 0.0f;
                    v1 = math.transform(worldToLocal, worldPos);
                    v0 = math.transform(worldToLocal, oldWorldPos);
                }

                oldVertexState[i] = v0;
                currentVertexState[i] = v1;
            }
        }
    };

    protected override void OnDestroy()
    {
        foreach (ClothBarSimEcs barSimEcs in s_MeshToBarSimLookup.Values)
        {
            barSimEcs.constraints.Dispose();
            barSimEcs.pinState.Dispose();
        }

        if (jobHandles.Length > 0)
	{
	    jobHandles.Dispose();
	}
    }

    protected override void OnCreate()
    {
        simGroup = GetEntityQuery(new EntityQueryDesc
        {
            All = new []
            {
                ComponentType.ReadOnly<ClothBarSimEcs>(),
                typeof(RenderMesh),
                ComponentType.ReadOnly<LocalToWorld>()
            }
        });

        PotentiallyResizeBecauseNewEntityHighwater((int)EntityHighwater.kInitial);
    }

    private void PotentiallyResizeBecauseNewEntityHighwater(int newHighwater)
    {
        if (newHighwater > jobHandles.Length)
        {
            if (jobHandles.Length > 0)
                jobHandles.Dispose();

            jobHandles = new NativeArray<JobHandle>(newHighwater, Allocator.Persistent);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        JobHandle combinedJobHandle = inputDeps;
	
        int entityCount = simGroup.CalculateEntityCount();
        if (entityCount > 0)
        {
            PotentiallyResizeBecauseNewEntityHighwater(entityCount);

            Entities.WithoutBurst().ForEach((RenderMesh renderMesh,
                                             ref DynamicBuffer<VertexStateCurrentElement> currentVertexState) =>
            {
                // jiv - RenderMesh is a shared component. We happen to know that there's a 1-1
                //       relationship between entities and RenderMesh instances, so it's not
                //       actually causing an error, but updating the mesh vertices per instance
                //       seems incorrect.
                renderMesh.mesh.SetVertices(currentVertexState.Reinterpret<Vector3>().AsNativeArray());
            }).Run();

            Entities.WithoutBurst().ForEach((int entityInQueryIndex,
                                             RenderMesh renderMesh,
                                             ref DynamicBuffer<VertexStateCurrentElement> currentVertexState,
                                             ref DynamicBuffer<VertexStateOldElement> oldVertexState,
                                             in ClothBarSimEcs clothBarSimEcs,
                                             in LocalToWorld localToWorld) =>
            {
                float4x4 worldToLocal = math.inverse(localToWorld.Value);

                NativeArray<float3> vertices = currentVertexState.Reinterpret<float3>().AsNativeArray();
                int vLength = clothBarSimEcs.pins.Length;

                // update vertices according to length constraints.  This process is serial
                // due to the nature of the dependencies between vertices.
                ClothBarSimJob clothBarSimJob = new ClothBarSimJob {
                    vertices = vertices,
                    bars = clothBarSimEcs.bars,
                    barLengths = clothBarSimEcs.barLengths,
                    pins = clothBarSimEcs.pins
                };

                JobHandle clothBarSimJobHandle = clothBarSimJob.Schedule(inputDeps);

                // apply gravity, bound (loosely) to +y halfspace, tick vertex state
                ClothSimVertexJob0 clothSimVertexJob0 = new ClothSimVertexJob0 {
                    pins = clothBarSimEcs.pins,
                    localToWorld = localToWorld.Value,
                    worldToLocal = worldToLocal,
                    gravity = math.mul(worldToLocal, worldGravity).xyz,
                    currentVertexState = vertices,
                    oldVertexState = oldVertexState.Reinterpret<float3>().AsNativeArray()
                };

                JobHandle simVertexJobHandle = clothSimVertexJob0.ScheduleBatch(vLength, vLength/SystemInfo.processorCount, clothBarSimJobHandle);
                jobHandles[entityInQueryIndex] = JobHandle.CombineDependencies(clothBarSimJobHandle, simVertexJobHandle);
            }).Run();
            UnityEngine.Profiling.Profiler.EndSample();

            UnityEngine.Profiling.Profiler.BeginSample("Cloth Job setup");

            // jiv
            // loop alone: ~0.43ms
            // ClothBarSimJob: ~1.04ms
            // ClothBarSimJob + instantiate but don't schedule ClothSimVertexJob0: 0.78
	    Entities
		.WithoutBurst()
		.ForEach((int entityInQueryIndex,
			  ref DynamicBuffer<VertexStateCurrentElement> currentVertexState,
			  ref DynamicBuffer<VertexStateOldElement> oldVertexState,
			  in ClothBarSimEcs clothBarSimEcs,
			  in LocalToWorld localToWorld,
			  in ClothInstance clothInstanceData) =>
		{
		    // can't hoist, will generate il2cpp codegen error about other omitted captures
		    float4 worldGravity = new float4(-Vector3.up * Time.DeltaTime*Time.DeltaTime, 0.0f);

		    NativeArray<float3> vertices = currentVertexState.Reinterpret<float3>().AsNativeArray();
		    NativeArray<float3> oldVertices = oldVertexState.Reinterpret<float3>().AsNativeArray();
		    int vLength = clothBarSimEcs.pinState.Length;

		    // update vertices according to length constraints.  This process is serial
		    // due to the nature of the dependencies between vertices.
		    ClothBarSimJob clothBarSimJob = new ClothBarSimJob {
			vertices = vertices,
			constraints = clothBarSimEcs.constraints
		    };

		    // apply gravity, bound (loosely) to +y halfspace, tick vertex state
		    ClothSimVertexJob0 clothSimVertexJob0 = new ClothSimVertexJob0 {
			pins = clothBarSimEcs.pinState,
			localToWorld = localToWorld.Value,
			worldToLocal = clothInstanceData.worldToLocalMatrix,
			localY0 = clothInstanceData.localY0,
			gravity = math.mul(clothInstanceData.worldToLocalMatrix, worldGravity).xyz,
			currentVertexState = vertices,
			oldVertexState = oldVertices
		    };

		    jobHandles[entityInQueryIndex] = clothSimVertexJob0.ScheduleBatch(vLength,
										      vLength/SystemInfo.processorCount,
										      clothBarSimJob.Schedule(inputDeps));
		}).Run();

            UnityEngine.Profiling.Profiler.EndSample();

            combinedJobHandle = JobHandle.CombineDependencies(new NativeSlice<JobHandle>(jobHandles, 0, entityCount));
        }

        return combinedJobHandle;
    }
}
