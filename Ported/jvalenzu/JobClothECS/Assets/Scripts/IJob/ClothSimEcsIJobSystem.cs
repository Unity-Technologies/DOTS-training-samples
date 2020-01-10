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

[UpdateAfter(typeof(ClothSimRenderMesh))]
public class ClothSimEcsIJobSystem : JobComponentSystem
{
    static Dictionary<Mesh, ClothBarSimEcs> s_MeshToBarSimLookup = new Dictionary<Mesh, ClothBarSimEcs>();

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
    struct ClothSimVertexIJob : IJob {
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

        public void Execute() {
            for (int i=0,n=pins.Length; i<n; ++i)
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
                    
                    v0 = math.transform(worldToLocal, oldWorldPos);
                    v1 = math.transform(worldToLocal, worldPos);
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
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        JobHandle combinedJobHandle = inputDeps;
        
        UnityEngine.Profiling.Profiler.BeginSample("ClothSetup");

        Entities.WithoutBurst().ForEach((ref DynamicBuffer<VertexStateCurrentElement> currentVertexState,
                                         ref DynamicBuffer<VertexStateOldElement> oldVertexState,
                                         in ClothBarSimEcs clothBarSimEcs,
                                         in ClothInstanceIJobIJob clothInstance,
                                         in LocalToWorld localToWorld) =>
        {
            // can't hoist, will generate il2cpp codegen error about other omitted captures
            float4 worldGravity = new float4(-Vector3.up * Time.DeltaTime*Time.DeltaTime, 0.0f);

            NativeArray<float3> vertices = currentVertexState.Reinterpret<float3>().AsNativeArray();
            int vLength = clothBarSimEcs.pinState.Length;

            // update vertices according to length constraints.  This process is serial
            // due to the nature of the dependencies between vertices.
            ClothBarSimJob clothBarSimJob = new ClothBarSimJob {
                vertices = vertices,
                constraints = clothBarSimEcs.constraints
            };

            JobHandle clothBarSimJobHandle = clothBarSimJob.Schedule(inputDeps);

            // apply gravity, bound (loosely) to +y halfspace, tick vertex state
            ClothSimVertexIJob clothSimVertexJob = new ClothSimVertexIJob {
                pins = clothBarSimEcs.pinState,
                localToWorld = localToWorld.Value,
                worldToLocal = clothInstance.worldToLocalMatrix,
                localY0 = clothInstance.localY0,
                gravity = math.mul(clothInstance.worldToLocalMatrix, worldGravity).xyz,
                currentVertexState = vertices,
                oldVertexState = oldVertexState.Reinterpret<float3>().AsNativeArray()
            };

            combinedJobHandle = JobHandle.CombineDependencies(combinedJobHandle, clothSimVertexJob.Schedule(clothBarSimJobHandle));
        }).Run();

        UnityEngine.Profiling.Profiler.EndSample();

        return combinedJobHandle;
    }
}

